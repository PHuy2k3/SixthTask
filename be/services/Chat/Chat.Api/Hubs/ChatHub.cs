using Chat.Biz.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Api.Hubs;

[Authorize]
public class ChatHub(IChatService biz) : Hub
{
    // Client gọi: joinConversation(conversationId)
    public Task JoinConversation(string conversationId)
        => Groups.AddToGroupAsync(Context.ConnectionId, conversationId);

    public Task LeaveConversation(string conversationId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);

    // (Optional) Client dùng hub để send luôn
    public async Task SendToConversation(string conversationId, string content)
    {
        var meId = Context.User!.GetUserId();
        var meName = Context.User!.GetUserName();

        var msg = await biz.SendMessageAsync(meId, meName, Guid.Parse(conversationId), content);

        // push cho tất cả trong group conversation
        await Clients.Group(conversationId).SendAsync("message:new", msg);
    }
}
