namespace Identity.Data;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}
