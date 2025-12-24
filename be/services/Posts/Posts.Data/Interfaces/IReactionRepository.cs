using Posts.Data.Model.Entities;

namespace Posts.Data.Interfaces;

public interface IReactionRepository
{
    Task<PostReaction?> GetMineAsync(Guid postId, Guid userId);
    Task AddAsync(PostReaction reaction);
    Task UpdateAsync(PostReaction reaction);
    Task RemoveAsync(PostReaction reaction);
    Task<List<(byte Type, int Count)>> GetCountsAsync(Guid postId);
}
