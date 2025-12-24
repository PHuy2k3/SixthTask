using Identity.Biz;
using Identity.Biz.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService biz) : ControllerBase
{
    [HttpPost("register")]
    public Task<AuthResult> Register([FromBody] RegisterReq req) => biz.RegisterAsync(req);

    [HttpPost("login")]
    public Task<AuthResult> Login([FromBody] LoginReq req) => biz.LoginAsync(req);

    [HttpPost("refresh")]
    public Task<AuthResult> Refresh([FromBody] RefreshReq req) => biz.RefreshAsync(req);

    [Authorize]
    [HttpPost("logout-all")]
    public Task LogoutAll()
    {
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        var userId = Guid.Parse(sub!);
        return biz.LogoutAllAsync(userId);
    }
}
