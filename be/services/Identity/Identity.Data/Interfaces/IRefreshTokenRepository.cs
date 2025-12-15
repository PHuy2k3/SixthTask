using Identity.Data.Model.Entities;

namespace Identity.Data;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    Task<RefreshToken?> FindValidByHashAsync(string tokenHash);
    Task RevokeAllAsync(Guid userId);
}
