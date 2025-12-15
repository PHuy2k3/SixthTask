using Identity.Data.Model.Entities;

namespace Identity.Data;

public interface IUserRepository
{
    Task<User?> FindByUserNameAsync(string userName);
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByIdAsync(Guid id);
    Task AddAsync(User user);
}
