using Microsoft.EntityFrameworkCore;
using Notifications.Data.Model.Entities;

namespace Notifications.Data;

public class NotificationRepository(NotificationsDbContext db) : INotificationRepository
{
    public Task AddAsync(Notification n)
    {
        db.Notifications.Add(n);
        return Task.CompletedTask;
    }

    public Task<List<Notification>> GetByUserAsync(Guid userId, int size)
        => db.Notifications
            .Where(x => x.RecipientUserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(size)
            .ToListAsync();

    public async Task MarkAsReadAsync(Guid id)
    {
        var n = await db.Notifications.FirstOrDefaultAsync(x => x.Id == id);
        if (n != null) n.IsRead = true;
    }
}
