using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.Biz.Security;

public sealed class JwtIssuer(IJwtOptions opt)
{
    public (string token, DateTime expiresAt) Issue(Guid userId, string userName)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(opt.AccessTokenMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim("username", userName),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opt.JwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: opt.Issuer,
            audience: opt.Audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(jwt), expires);
    }
}

public interface IJwtOptions
{
    string JwtKey { get; }
    string Issuer { get; }
    string Audience { get; }
    int AccessTokenMinutes { get; }
    int RefreshTokenDays { get; }
}
