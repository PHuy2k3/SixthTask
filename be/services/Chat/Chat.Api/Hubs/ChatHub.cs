using Chat.Biz.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    // client gọi để join group theo conversationId
    public Task JoinConversation(string conversationId)
        => Groups.AddToGroupAsync(Context.ConnectionId, conversationId);

    public Task LeaveConversation(string conversationId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);

    // ========== WebRTC Signaling ==========
    public Task SendOffer(string conversationId, object offer)
        => Clients.Group(conversationId).SendAsync("call:offer", new
        {
            fromUserId = Context.User!.GetUserId(),
            fromUserName = Context.User!.GetUserName(),
            offer
        });

    public Task SendAnswer(string conversationId, object answer)
        => Clients.Group(conversationId).SendAsync("call:answer", new
        {
            fromUserId = Context.User!.GetUserId(),
            fromUserName = Context.User!.GetUserName(),
            answer
        });

    public Task SendIceCandidate(string conversationId, object candidate)
        => Clients.Group(conversationId).SendAsync("call:ice", new
        {
            fromUserId = Context.User!.GetUserId(),
            candidate
        });

    public override Task OnConnectedAsync()
        => base.OnConnectedAsync();
}
