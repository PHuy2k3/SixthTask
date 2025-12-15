using Identity.Data.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Data;

public class RefreshTokenRepository(IdentityDbContext db) : IRefreshTokenRepository
{
    public Task AddAsync(RefreshToken token)
    {
        db.RefreshTokens.Add(token);
        return Task.CompletedTask;
    }

    public Task<RefreshToken?> FindValidByHashAsync(string tokenHash)
    {
        var now = DateTime.UtcNow;
        return db.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash && !x.IsRevoked && x.ExpiresAt > now);
    }

    public async Task RevokeAllAsync(Guid userId)
    {
        var tokens = await db.RefreshTokens.Where(x => x.UserId == userId && !x.IsRevoked).ToListAsync();
        foreach (var t in tokens) t.IsRevoked = true;
    }
}
