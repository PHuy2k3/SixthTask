using Microsoft.EntityFrameworkCore;
using Posts.Data.Model.Entities;

namespace Posts.Data;

public class PostRepository(PostsDbContext db) : IPostRepository
{
    public Task AddAsync(Post post)
    {
        db.Posts.Add(post);
        return Task.CompletedTask;
    }

    public Task<List<Post>> GetLatestAsync(int size)
        => db.Posts.OrderByDescending(x => x.CreatedAt).Take(size).ToListAsync();
}
