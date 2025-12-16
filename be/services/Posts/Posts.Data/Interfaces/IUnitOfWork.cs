namespace Posts.Data.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}
