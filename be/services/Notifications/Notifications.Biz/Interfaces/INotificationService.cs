using Notifications.Biz.Model;

namespace Notifications.Biz;

public interface INotificationService
{
    Task<NotificationDto> CreateAsync(CreateNotificationReq req);
    Task<List<NotificationDto>> GetMineAsync(Guid userId, int size);
    Task MarkReadAsync(Guid id);
}
