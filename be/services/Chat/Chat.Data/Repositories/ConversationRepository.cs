using Chat.Data.Interfaces;
using Chat.Data.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chat.Data.Repositories;

public class ConversationRepository(ChatDbContext db) : IConversationRepository
{
    public Task<Conversation?> GetAsync(Guid id)
        => db.Conversations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    public Task<Conversation?> GetTrackAsync(Guid id)
        => db.Conversations.FirstOrDefaultAsync(x => x.Id == id);

    public Task<Conversation?> FindByPairAsync(Guid userMin, Guid userMax)
        => db.Conversations.FirstOrDefaultAsync(x => x.UserMin == userMin && x.UserMax == userMax);

    public Task AddAsync(Conversation c) => db.Conversations.AddAsync(c).AsTask();

    public Task<List<Conversation>> GetMineAsync(Guid meId, int size)
        => db.Conversations.AsNoTracking()
            .Where(x => x.UserAId == meId || x.UserBId == meId)
            .OrderByDescending(x => x.LastMessageAt ?? x.CreatedAt)
            .Take(size <= 0 || size > 100 ? 30 : size)
            .ToListAsync();

    public Task SaveAsync() => db.SaveChangesAsync();
}
