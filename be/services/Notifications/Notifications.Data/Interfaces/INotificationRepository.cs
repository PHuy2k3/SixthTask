using Notifications.Data.Model.Entities;

namespace Notifications.Data;

public interface INotificationRepository
{
    Task AddAsync(Notification n);
    Task<List<Notification>> GetByUserAsync(Guid userId, int size);
    Task MarkAsReadAsync(Guid id);
}
