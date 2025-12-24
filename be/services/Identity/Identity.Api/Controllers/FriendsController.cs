using Identity.Data;
using Identity.Data.Model.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Kernel;
using Shared.Kernel.Claims;

[ApiController]
[Route("api/friends")]
public class FriendsController : ControllerBase
{
    private readonly IdentityDbContext _db;
    public FriendsController(IdentityDbContext db) => _db = db;

    // ===================== STATUS =====================
    [Authorize]
    [HttpGet("status/{userId:guid}")]
    public async Task<IActionResult> Status(Guid userId)
    {
        var me = User.GetUserId();
        if (me == userId) return Ok(new { status = "me" });

        var req1 = await _db.FriendRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.FromUserId == me && x.ToUserId == userId);

        var req2 = await _db.FriendRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.FromUserId == userId && x.ToUserId == me);

        if ((req1?.Status == 1) || (req2?.Status == 1))
            return Ok(new { status = "friends" });

        if (req1?.Status == 0) return Ok(new { status = "pending_out" });
        if (req2?.Status == 0) return Ok(new { status = "pending_in" });

        return Ok(new { status = "none" });
    }

    // ===================== REQUEST =====================
    [Authorize]
    [HttpPost("request/{userId:guid}")]
    public async Task<IActionResult> Request(Guid userId)
    {
        var me = User.GetUserId();
        if (me == userId)
            return BadRequest("Cannot friend yourself");

        var exists = await _db.FriendRequests.AnyAsync(x =>
            x.FromUserId == me && x.ToUserId == userId);

        if (!exists)
        {
            _db.FriendRequests.Add(new FriendRequest
            {
                Id = Guid.NewGuid(),
                FromUserId = me,
                ToUserId = userId,
                Status = 0,
                CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
        }

        return Ok(new { ok = true });
    }

    // ===================== ACCEPT =====================
    [Authorize]
    [HttpPost("accept/{userId:guid}")]
    public async Task<IActionResult> Accept(Guid userId)
    {
        var me = User.GetUserId();

        var req = await _db.FriendRequests.FirstOrDefaultAsync(x =>
            x.FromUserId == userId &&
            x.ToUserId == me &&
            x.Status == 0);

        if (req == null)
            return BadRequest("No pending request");

        req.Status = 1;
        await _db.SaveChangesAsync();
        return Ok(new { ok = true });
    }

    // ===================== LIST FRIENDS =====================
    [Authorize]
    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        var me = User.GetUserId();

        // 1️⃣ Lấy ID bạn bè
        var friendIds = await _db.FriendRequests
            .AsNoTracking()
            .Where(x =>
                x.Status == 1 &&
                (x.FromUserId == me || x.ToUserId == me))
            .Select(x => x.FromUserId == me ? x.ToUserId : x.FromUserId)
            .ToListAsync();

        if (friendIds.Count == 0)
            return Ok(new List<object>());

        // 2️⃣ Lấy user (KHÔNG new DTO trong LINQ)
        var users = await _db.Users
            .AsNoTracking()
            .Where(u => friendIds.Contains(u.Id))
            .OrderBy(u => u.UserName)
            .ToListAsync();

        // 3️⃣ Map sang DTO sau
        var result = users.Select(u => new
        {
            id = u.Id,
            userName = u.UserName
        });

        return Ok(result);
    }
}
