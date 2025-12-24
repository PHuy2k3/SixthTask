using Microsoft.EntityFrameworkCore;
using Posts.Data.Interfaces;
using Posts.Data.Model.Entities;

namespace Posts.Data.Repositories;

public class ReactionRepository : IReactionRepository
{
    private readonly PostsDbContext _db;
    public ReactionRepository(PostsDbContext db) => _db = db;

    public Task<PostReaction?> GetMineAsync(Guid postId, Guid userId)
        => _db.PostReactions.FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);

    public async Task AddAsync(PostReaction reaction)
    {
        await _db.PostReactions.AddAsync(reaction);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(PostReaction reaction)
    {
        _db.PostReactions.Update(reaction);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(PostReaction reaction)
    {
        _db.PostReactions.Remove(reaction);
        await _db.SaveChangesAsync();
    }

    public async Task<List<(byte Type, int Count)>> GetCountsAsync(Guid postId)
        => await _db.PostReactions
            .Where(x => x.PostId == postId)
            .GroupBy(x => x.Type)
            .Select(g => new ValueTuple<byte, int>(g.Key, g.Count()))
            .ToListAsync();
}
