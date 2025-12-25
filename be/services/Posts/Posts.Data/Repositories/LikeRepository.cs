using Microsoft.EntityFrameworkCore;
using Posts.Data.Interfaces;
using Posts.Data.Model.Entities;

namespace Posts.Data.Repositories;

public class LikeRepository : ILikeRepository
{
    private readonly PostsDbContext _db;

    public LikeRepository(PostsDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(PostLike like)
    {
        _db.Likes.Add(like);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(Guid postId, Guid userId)
    {
        var e = await _db.Likes.FirstAsync(x => x.PostId == postId && x.UserId == userId);
        _db.Likes.Remove(e);
        await _db.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(Guid postId, Guid userId)
        => _db.Likes.AnyAsync(x => x.PostId == postId && x.UserId == userId);

    public Task<int> CountByPostAsync(Guid postId)
        => _db.Likes.CountAsync(x => x.PostId == postId);
    public async Task RemoveAllByPostAsync(Guid postId)
    {
        var items = await _db.Likes.Where(x => x.PostId == postId).ToListAsync();
        if (items.Count == 0) return;

        _db.Likes.RemoveRange(items);
        await _db.SaveChangesAsync();
    }
}
