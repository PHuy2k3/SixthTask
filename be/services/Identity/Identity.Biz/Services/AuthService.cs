using Identity.Biz.Model;
using Identity.Biz.Security;
using Identity.Data;
using Identity.Data.Model.Entities;
using Shared.Kernel.Exceptions;

namespace Identity.Biz;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refresh;
    private readonly IUnitOfWork _uow;
    private readonly JwtIssuer _jwt;
    private readonly IJwtOptions _opt;

    public AuthService(IUserRepository users, IRefreshTokenRepository refresh, IUnitOfWork uow, JwtIssuer jwt, IJwtOptions opt)
    {
        _users = users; _refresh = refresh; _uow = uow; _jwt = jwt; _opt = opt;
    }

    public async Task<AuthResult> RegisterAsync(RegisterReq req)
    {
        if (string.IsNullOrWhiteSpace(req.UserName) || req.UserName.Length < 3)
            throw new BizException("USERNAME_INVALID", "UserName tối thiểu 3 ký tự");

        if (string.IsNullOrWhiteSpace(req.Email) || !req.Email.Contains('@'))
            throw new BizException("EMAIL_INVALID", "Email không hợp lệ");

        if (string.IsNullOrWhiteSpace(req.Password) || req.Password.Length < 6)
            throw new BizException("PASSWORD_INVALID", "Password tối thiểu 6 ký tự");

        if (await _users.FindByUserNameAsync(req.UserName) != null)
            throw new BizException("USERNAME_EXISTS", "UserName đã tồn tại", 409);

        if (await _users.FindByEmailAsync(req.Email) != null)
            throw new BizException("EMAIL_EXISTS", "Email đã tồn tại", 409);

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = req.UserName.Trim(),
            Email = req.Email.Trim().ToLowerInvariant(),
            PasswordHash = PasswordHasher.Hash(req.Password),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _users.AddAsync(user);

        var auth = await IssueTokensAsync(user);
        await _uow.SaveChangesAsync();
        return auth;
    }

    public async Task<AuthResult> LoginAsync(LoginReq req)
    {
        var key = req.UserNameOrEmail.Trim();
        var user = key.Contains('@')
            ? await _users.FindByEmailAsync(key.ToLowerInvariant())
            : await _users.FindByUserNameAsync(key);

        if (user == null || !user.IsActive)
            throw new BizException("LOGIN_FAILED", "Sai tài khoản hoặc mật khẩu", 401);

        if (!PasswordHasher.Verify(req.Password, user.PasswordHash))
            throw new BizException("LOGIN_FAILED", "Sai tài khoản hoặc mật khẩu", 401);

        var auth = await IssueTokensAsync(user);
        await _uow.SaveChangesAsync();
        return auth;
    }

    public async Task<AuthResult> RefreshAsync(RefreshReq req)
    {
        if (string.IsNullOrWhiteSpace(req.RefreshToken))
            throw new BizException("REFRESH_INVALID", "Refresh token rỗng", 401);

        var hash = TokenHasher.Sha256(req.RefreshToken);
        var token = await _refresh.FindValidByHashAsync(hash);
        if (token == null)
            throw new BizException("REFRESH_INVALID", "Refresh token không hợp lệ", 401);

        // rotate: revoke token cũ
        token.IsRevoked = true;

        var user = await _users.FindByIdAsync(token.UserId);
        if (user == null || !user.IsActive)
            throw new BizException("REFRESH_INVALID", "User không hợp lệ", 401);

        var auth = await IssueTokensAsync(user);
        await _uow.SaveChangesAsync();
        return auth;
    }

    public async Task LogoutAllAsync(Guid userId)
    {
        await _refresh.RevokeAllAsync(userId);
        await _uow.SaveChangesAsync();
    }

    private async Task<AuthResult> IssueTokensAsync(User user)
    {
        var (access, accessExp) = _jwt.Issue(user.Id, user.UserName);

        var refreshPlain = TokenHasher.NewRefreshToken();
        var refreshHash = TokenHasher.Sha256(refreshPlain);

        await _refresh.AddAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_opt.RefreshTokenDays),
            IsRevoked = false
        });

        return new AuthResult(access, refreshPlain, accessExp, user.UserName);
    }
}
