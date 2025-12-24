using Chat.Data.Interfaces;
using Chat.Data.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chat.Data.Repositories;

public class MessageRepository(ChatDbContext db) : IMessageRepository
{
    public Task AddAsync(Message m) => db.Messages.AddAsync(m).AsTask();

    public Task<List<Message>> GetByConversationAsync(Guid conversationId, int size)
        => db.Messages.AsNoTracking()
            .Where(x => x.ConversationId == conversationId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(size <= 0 || size > 200 ? 50 : size)
            .OrderBy(x => x.CreatedAt) // trả về tăng dần cho UI dễ render
            .ToListAsync();

    public Task<int> CountUnreadAsync(Guid conversationId, Guid meId)
        => db.Messages.AsNoTracking()
            .Where(x => x.ConversationId == conversationId && x.SenderId != meId && !x.IsRead)
            .CountAsync();

    public async Task MarkReadAsync(Guid conversationId, Guid meId)
    {
        var q = db.Messages
            .Where(x => x.ConversationId == conversationId && x.SenderId != meId && !x.IsRead);

        await q.ExecuteUpdateAsync(s => s.SetProperty(x => x.IsRead, true));
    }

    public Task SaveAsync() => db.SaveChangesAsync();
}
