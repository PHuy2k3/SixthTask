using Chat.Biz.Model;

namespace Chat.Biz.Interfaces;

public interface IChatService
{
    Task<ConversationDto> CreateOrGetConversationAsync(Guid meId, string meName, Guid otherId, string otherName);
    Task<List<ConversationDto>> GetMyConversationsAsync(Guid meId, int size);
    Task<List<MessageDto>> GetMessagesAsync(Guid meId, Guid conversationId, int size);

    Task<MessageDto> SendMessageAsync(Guid meId, string meName, Guid conversationId, string content);
    Task MarkReadAsync(Guid meId, Guid conversationId);
}
