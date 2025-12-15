namespace Identity.Data;

public class UnitOfWork(IdentityDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync() => db.SaveChangesAsync();
}
