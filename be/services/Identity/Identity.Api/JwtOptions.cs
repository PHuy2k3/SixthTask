using Identity.Biz.Security;

namespace Identity.Api;

public sealed class JwtOptions : IJwtOptions
{
    public string JwtKey { get; init; } = "";
    public string Issuer { get; init; } = "";
    public string Audience { get; init; } = "";
    public int AccessTokenMinutes { get; init; }
    public int RefreshTokenDays { get; init; }
}
