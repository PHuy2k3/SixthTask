using Chat.Data.Model.Entities;

namespace Chat.Data.Interfaces;

public interface IConversationRepository
{
    Task<Conversation?> GetAsync(Guid id);           // NoTracking
    Task<Conversation?> GetTrackAsync(Guid id);      // Tracking
    Task<Conversation?> FindByPairAsync(Guid userMin, Guid userMax); // Tracking
    Task AddAsync(Conversation c);
    Task<List<Conversation>> GetMineAsync(Guid meId, int size);
    Task SaveAsync();
}
