using Chat.Data.Model.Entities;

namespace Chat.Data.Interfaces;

public interface IMessageRepository
{
    Task AddAsync(Message m);
    Task<List<Message>> GetByConversationAsync(Guid conversationId, int size);
    Task<int> CountUnreadAsync(Guid conversationId, Guid meId);
    Task MarkReadAsync(Guid conversationId, Guid meId);
    Task SaveAsync();
}
