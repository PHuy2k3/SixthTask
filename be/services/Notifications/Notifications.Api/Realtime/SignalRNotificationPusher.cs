using Microsoft.AspNetCore.SignalR;
using Notifications.Api.Hubs;
using Notifications.Biz.Model;

namespace Notifications.Api.Realtime;

public class SignalRNotificationPusher(IHubContext<NotificationHub> hub) : INotificationPusher
{
    public Task PushAsync(Guid recipientUserId, NotificationDto dto)
        => hub.Clients.Group($"user:{recipientUserId}").SendAsync("notify", dto);
}
