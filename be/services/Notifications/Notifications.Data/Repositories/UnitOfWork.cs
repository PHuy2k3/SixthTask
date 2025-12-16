namespace Notifications.Data;

public class UnitOfWork(NotificationsDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync() => db.SaveChangesAsync();
}
