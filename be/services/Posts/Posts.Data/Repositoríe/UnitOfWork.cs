namespace Posts.Data;

public class UnitOfWork(PostsDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync() => db.SaveChangesAsync();
}
