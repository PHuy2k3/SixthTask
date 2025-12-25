using Microsoft.EntityFrameworkCore;
using Posts.Data.Interfaces;
using Posts.Data.Model.Entities;

namespace Posts.Data.Repositories;

public class PostMediaRepository : IPostMediaRepository
{
    private readonly PostsDbContext _db;
    public PostMediaRepository(PostsDbContext db) { _db = db; }

    public async Task AddManyAsync(List<PostMedia> items)
    {
        _db.PostMedia.AddRange(items);
        await _db.SaveChangesAsync();
    }

    public Task<List<string>> GetUrlsByPostAsync(Guid postId)
        => _db.PostMedia
            .Where(x => x.PostId == postId)
            .OrderBy(x => x.CreatedAt)
            .Select(x => x.Url)
            .ToListAsync();
    public async Task DeleteByPostAsync(Guid postId)
    {
        var items = await _db.PostMedia.Where(x => x.PostId == postId).ToListAsync();
        if (items.Count == 0) return;

        _db.PostMedia.RemoveRange(items);
        await _db.SaveChangesAsync();
    }
}
