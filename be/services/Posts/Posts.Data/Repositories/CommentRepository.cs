using Microsoft.EntityFrameworkCore;
using Posts.Data.Interfaces;
using Posts.Data.Model.Entities;

namespace Posts.Data.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly PostsDbContext _db;

    public CommentRepository(PostsDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(PostComment cmt)
    {
        _db.Comments.Add(cmt);
        await _db.SaveChangesAsync();
    }

    public Task<int> CountByPostAsync(Guid postId)
        => _db.Comments.CountAsync(x => x.PostId == postId);

    public Task<List<PostComment>> GetByPostAsync(Guid postId)
        => _db.Comments
            .Where(x => x.PostId == postId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    public async Task<PostComment?> GetAsync(Guid id)
    => await _db.Comments.FirstOrDefaultAsync(x => x.Id == id);

    public async Task UpdateAsync(PostComment cmt)
    {
        _db.Comments.Update(cmt);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(PostComment cmt)
    {
        _db.Comments.Remove(cmt);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteByPostAsync(Guid postId)
    {
        var items = await _db.Comments.Where(x => x.PostId == postId).ToListAsync();
        if (items.Count == 0) return;
        _db.Comments.RemoveRange(items);
        await _db.SaveChangesAsync();
    }

}
