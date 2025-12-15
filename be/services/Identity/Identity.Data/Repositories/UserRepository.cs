using Identity.Data.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Data;

public class UserRepository(IdentityDbContext db) : IUserRepository
{
    public Task<User?> FindByUserNameAsync(string userName)
        => db.Users.FirstOrDefaultAsync(x => x.UserName == userName);

    public Task<User?> FindByEmailAsync(string email)
        => db.Users.FirstOrDefaultAsync(x => x.Email == email);

    public Task<User?> FindByIdAsync(Guid id)
        => db.Users.FirstOrDefaultAsync(x => x.Id == id);

    public Task AddAsync(User user)
    {
        db.Users.Add(user);
        return Task.CompletedTask;
    }
}
