using Identity.Biz.Model;
using Microsoft.IdentityModel.Tokens;
using Shared.Kernel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.Biz.Security;

public interface IJwtService
{
    string CreateAccessToken(Guid userId, string userName);
}

public class JwtService(IJwtOptions opt) : IJwtService
{
    public string CreateAccessToken(Guid userId, string userName)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, userName),             // ✅ QUAN TRỌNG
            new Claim("username", userName),                  // optional
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opt.JwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: opt.Issuer,
            audience: opt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(opt.AccessTokenMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
