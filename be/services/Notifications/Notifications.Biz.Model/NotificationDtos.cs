namespace Notifications.Biz.Model;

public sealed record CreateNotificationReq(
    Guid RecipientUserId,
    Guid ActorUserId,
    string ActorUserName,
    string Type,
    Guid PostId,
    string? Content
);

public sealed record NotificationDto(
    Guid Id,
    Guid RecipientUserId,
    Guid ActorUserId,
    string ActorUserName,
    string Type,
    Guid PostId,
    string? Content,
    bool IsRead,
    DateTime CreatedAt
);

public interface INotificationPusher
{
    Task PushAsync(Guid recipientUserId, NotificationDto dto);
}
