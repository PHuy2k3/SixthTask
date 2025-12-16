using Posts.Biz.Interfaces;
using Posts.Biz.Model;
using Posts.Data.Interfaces;
using Posts.Data.Model.Entities;

namespace Posts.Biz;

public class PostService : IPostService
{
    private readonly IPostRepository _posts;
    private readonly ILikeRepository _likes;
    private readonly ICommentRepository _comments;
    private readonly IPostMediaRepository _media;

    public PostService(
        IPostRepository posts,
        ILikeRepository likes,
        ICommentRepository comments,
        IPostMediaRepository media)
    {
        _posts = posts;
        _likes = likes;
        _comments = comments;
        _media = media;
    }

    // ✅ Create post (có thể có imageUrls)
    public async Task<PostDto> CreateAsync(CreatePostReq req, Guid userId, string userName, List<string>? imageUrls = null)
    {
        if (string.IsNullOrWhiteSpace(req.Content) && (imageUrls is null || imageUrls.Count == 0))
            throw new Exception("Post must have content or images.");

        var post = new Post
        {
            Id = Guid.NewGuid(),
            AuthorId = userId,
            AuthorUserName = userName,
            Content = req.Content ?? "",
            Privacy = string.IsNullOrWhiteSpace(req.Privacy) ? "public" : req.Privacy,
            CreatedAt = DateTime.UtcNow
        };

        await _posts.AddAsync(post);

        if (imageUrls is { Count: > 0 })
        {
            var items = imageUrls
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(url => new PostMedia
                {
                    Id = Guid.NewGuid(),
                    PostId = post.Id,
                    Url = url,
                    CreatedAt = DateTime.UtcNow
                })
                .ToList();

            if (items.Count > 0)
                await _media.AddManyAsync(items);
        }

        return await BuildDtoAsync(post, userId);
    }

    public async Task<PostDto> ToggleLikeAsync(Guid postId, Guid userId)
    {
        if (await _likes.ExistsAsync(postId, userId))
            await _likes.RemoveAsync(postId, userId);
        else
            await _likes.AddAsync(new PostLike { PostId = postId, UserId = userId, CreatedAt = DateTime.UtcNow });

        var p = await _posts.GetAsync(postId) ?? throw new Exception("Post not found");
        return await BuildDtoAsync(p, userId);
    }

    public async Task<PostCommentDto> AddCommentAsync(Guid postId, AddCommentReq req, Guid userId, string userName)
    {
        if (string.IsNullOrWhiteSpace(req.Content))
            throw new Exception("Comment content is required.");

        var post = await _posts.GetAsync(postId);
        if (post is null) throw new Exception("Post not found");

        var cmt = new PostComment
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            UserId = userId,
            UserName = userName,
            Content = req.Content,
            CreatedAt = DateTime.UtcNow
        };

        await _comments.AddAsync(cmt);

        return new PostCommentDto(cmt.Id, cmt.PostId, cmt.UserId, cmt.UserName, cmt.Content, cmt.CreatedAt);
    }

    public async Task<List<PostDto>> GetLatestAsync(Guid meId, int size)
        => await BuildListAsync(await _posts.GetLatestAsync(Clamp(size)), meId);

    public async Task<List<PostDto>> GetMyPostsAsync(Guid meId, int size)
        => await BuildListAsync(await _posts.GetByAuthorAsync(meId, Clamp(size)), meId);

    public async Task<List<PostCommentDto>> GetCommentsAsync(Guid postId)
        => (await _comments.GetByPostAsync(postId))
            .Select(c => new PostCommentDto(c.Id, c.PostId, c.UserId, c.UserName, c.Content, c.CreatedAt))
            .ToList();

    // ----------------- helpers -----------------

    static int Clamp(int s) => s <= 0 || s > 100 ? 20 : s;

    async Task<List<PostDto>> BuildListAsync(List<Post> posts, Guid meId)
    {
        var list = new List<PostDto>(posts.Count);
        foreach (var p in posts)
            list.Add(await BuildDtoAsync(p, meId));
        return list;
    }

    async Task<PostDto> BuildDtoAsync(Post p, Guid meId)
    {
        var urls = await _media.GetUrlsByPostAsync(p.Id);

        var likeCount = await _likes.CountByPostAsync(p.Id);
        var commentCount = await _comments.CountByPostAsync(p.Id);
        var likedByMe = await _likes.ExistsAsync(p.Id, meId);

        return new PostDto(
            p.Id,
            p.AuthorId,
            p.AuthorUserName,
            p.Content,
            p.Privacy,
            p.CreatedAt,
            likeCount,
            commentCount,
            likedByMe,
            urls
        );
    }
}
