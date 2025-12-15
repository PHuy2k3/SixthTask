using Posts.Biz.Model;
using Posts.Data;
using Posts.Data.Model;
using Shared.Kernel;
using Posts.Data.Model.Entities;

namespace Posts.Biz;

public class PostService(IPostRepository repo, IUnitOfWork uow) : IPostService
{
    public async Task<PostDto> CreateAsync(CreatePostReq req, Guid userId)
    {
        // business validate
        if (string.IsNullOrWhiteSpace(req.Content))
            throw new BizException("POST_EMPTY", "Nội dung không được rỗng");

        if (req.Privacy is not ("public" or "friends" or "private"))
            throw new BizException("PRIVACY_INVALID", "Privacy không hợp lệ");

        var post = new Post
        {
            Id = Guid.NewGuid(),
            AuthorId = userId,
            Content = req.Content.Trim(),
            Privacy = req.Privacy,
            CreatedAt = DateTime.UtcNow
        };

        await repo.AddAsync(post);
        await uow.SaveChangesAsync();

        return new PostDto(post.Id, post.AuthorId, post.Content, post.Privacy, post.CreatedAt);
    }

    public async Task<List<PostDto>> GetLatestAsync(int size)
    {
        if (size is <= 0 or > 100) size = 20;

        var items = await repo.GetLatestAsync(size);
        return items.Select(p => new PostDto(p.Id, p.AuthorId, p.Content, p.Privacy, p.CreatedAt)).ToList();
    }
}
