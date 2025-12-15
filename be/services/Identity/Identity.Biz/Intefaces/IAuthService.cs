using Identity.Biz.Model;

namespace Identity.Biz;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterReq req);
    Task<AuthResult> LoginAsync(LoginReq req);
    Task<AuthResult> RefreshAsync(RefreshReq req);
    Task LogoutAllAsync(Guid userId);
}
