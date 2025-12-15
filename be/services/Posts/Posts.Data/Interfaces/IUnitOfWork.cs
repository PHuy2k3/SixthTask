namespace Posts.Data;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}
