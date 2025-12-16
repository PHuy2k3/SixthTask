using Notifications.Biz.Model;
using Notifications.Data;
using Notifications.Data.Model.Entities;

namespace Notifications.Biz;

public class NotificationService(
    INotificationRepository repo,
    IUnitOfWork uow,
    INotificationPusher pusher
) : INotificationService
{
    public async Task<NotificationDto> CreateAsync(CreateNotificationReq req)
    {
        var n = new Notification
        {
            Id = Guid.NewGuid(),
            RecipientUserId = req.RecipientUserId,
            ActorUserId = req.ActorUserId,
            ActorUserName = req.ActorUserName,
            Type = req.Type,
            PostId = req.PostId,
            Content = req.Content,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await repo.AddAsync(n);
        await uow.SaveChangesAsync();

        var dto = new NotificationDto(
            n.Id, n.RecipientUserId, n.ActorUserId, n.ActorUserName,
            n.Type, n.PostId, n.Content, n.IsRead, n.CreatedAt
        );

        await pusher.PushAsync(n.RecipientUserId, dto);
        return dto;
    }

    public async Task<List<NotificationDto>> GetMineAsync(Guid userId, int size)
    {
        if (size is <= 0 or > 100) size = 20;

        var items = await repo.GetByUserAsync(userId, size);
        return items.Select(n => new NotificationDto(
            n.Id, n.RecipientUserId, n.ActorUserId, n.ActorUserName,
            n.Type, n.PostId, n.Content, n.IsRead, n.CreatedAt
        )).ToList();
    }

    public async Task MarkReadAsync(Guid id)
    {
        await repo.MarkAsReadAsync(id);
        await uow.SaveChangesAsync();
    }
}
