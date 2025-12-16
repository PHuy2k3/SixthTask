namespace Identity.Biz.Model;

public sealed record RegisterReq(string UserName, string Email, string Password);
public sealed record LoginReq(string UserNameOrEmail, string Password);
public sealed record RefreshReq(string RefreshToken);

public sealed record AuthResult(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt, string UserName);

