using Chat.Api.Hubs;
using Chat.Biz.Interfaces;
using Chat.Biz.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Api.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController(
    IChatService biz,
    IHubContext<ChatHub> hub
) : ControllerBase
{
    [Authorize]
    [HttpPost("conversations")]
    public Task<ConversationDto> CreateOrGet([FromBody] CreateConversationReq req)
        => biz.CreateOrGetConversationAsync(User.GetUserId(), User.GetUserName(), req.OtherUserId, req.OtherUserName);

    [Authorize]
    [HttpGet("conversations")]
    public Task<List<ConversationDto>> MyConversations([FromQuery] int size = 30)
        => biz.GetMyConversationsAsync(User.GetUserId(), size);

    [Authorize]
    [HttpGet("messages/{conversationId:guid}")]
    public Task<List<MessageDto>> Messages(Guid conversationId, [FromQuery] int size = 50)
        => biz.GetMessagesAsync(User.GetUserId(), conversationId, size);

    [Authorize]
    [HttpPost("messages")]
    public async Task<MessageDto> Send([FromBody] SendMessageReq req)
    {
        var msg = await biz.SendMessageAsync(User.GetUserId(), User.GetUserName(), req.ConversationId, req.Content);

        // push realtime cho group = conversationId
        await hub.Clients.Group(req.ConversationId.ToString()).SendAsync("message:new", msg);

        // push cho người nhận theo userId (nếu muốn hiển thị badge)
        // => bên client join group vẫn đủ, cái này optional.
        return msg;
    }

    [Authorize]
    [HttpPut("read")]
    public async Task<object> MarkRead([FromBody] MarkReadReq req)
    {
        await biz.MarkReadAsync(User.GetUserId(), req.ConversationId);
        return new { ok = true };
    }
}
