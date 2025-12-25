using Microsoft.EntityFrameworkCore;
using Posts.Data.Interfaces;
using Posts.Data.Model.Entities;

namespace Posts.Data.Repositories;

public class PostRepository : IPostRepository
{
    private readonly PostsDbContext _db;

    public PostRepository(PostsDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Post p)
    {
        _db.Posts.Add(p);
        await _db.SaveChangesAsync();
    }

    public Task<Post?> GetAsync(Guid id)
        => _db.Posts.FirstOrDefaultAsync(x => x.Id == id);

    public Task<List<Post>> GetLatestAsync(int size)
        => _db.Posts
            .OrderByDescending(x => x.CreatedAt)
            .Take(size)
            .ToListAsync();

    public Task<List<Post>> GetByAuthorAsync(Guid authorId, int size)
        => _db.Posts
            .Where(x => x.AuthorId == authorId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(size)
            .ToListAsync();
    public async Task UpdateAsync(Post post)
    {
        _db.Posts.Update(post);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Post post)
    {
        _db.Posts.Remove(post);
        await _db.SaveChangesAsync();
    }

}
