namespace Notifications.Data;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}
