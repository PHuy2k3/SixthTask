using Posts.Data.Interfaces;

namespace Posts.Data.Repositories;
public class UnitOfWork(PostsDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync() => db.SaveChangesAsync();
}
