using Posts.Biz.Interfaces;
using Posts.Biz.Model;
using Posts.Data.Interfaces;
using Posts.Data.Model.Entities;

namespace Posts.Biz;

public class PostService : IPostService
{
    private readonly IPostRepository _posts;
    private readonly ICommentRepository _comments;
    private readonly IPostMediaRepository _media;
    private readonly IReactionRepository _reactions;
    private readonly ILikeRepository _likes;

    public PostService(
        IPostRepository posts,
        ICommentRepository comments,
        IPostMediaRepository media,
        IReactionRepository reactions,
        ILikeRepository likes)
    {
        _posts = posts;
        _comments = comments;
        _media = media;
        _reactions = reactions;
        _likes = likes;
    }

    static int Clamp(int s) => s <= 0 || s > 100 ? 20 : s;

    public async Task<PostDto> CreateAsync(CreatePostReq req, Guid userId, string userName, List<string>? imageUrls = null)
    {
        if (string.IsNullOrWhiteSpace(req.Content) && (imageUrls == null || imageUrls.Count == 0))
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

        if (imageUrls?.Count > 0)
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

    public async Task<PostDto> SetReactionAsync(Guid postId, byte type, Guid userId, string userName)
    {
        var post = await _posts.GetAsync(postId) ?? throw new Exception("Post not found");

        var mine = await _reactions.GetMineAsync(postId, userId);

        if (mine == null)
        {
            await _reactions.AddAsync(new PostReaction
            {
                Id = Guid.NewGuid(),
                PostId = postId,
                UserId = userId,
                UserName = userName,
                Type = type,
                CreatedAt = DateTime.UtcNow
            });
        }
        else if (mine.Type == type)
        {
            await _reactions.RemoveAsync(mine);
        }
        else
        {
            mine.Type = type;
            await _reactions.UpdateAsync(mine);
        }

        return await BuildDtoAsync(post, userId);
    }

    // ✅ COMMENT: ADD
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

    // ✅ COMMENT: GET
    public async Task<List<PostCommentDto>> GetCommentsAsync(Guid postId)
        => (await _comments.GetByPostAsync(postId))
            .Select(c => new PostCommentDto(c.Id, c.PostId, c.UserId, c.UserName, c.Content, c.CreatedAt))
            .ToList();

    public async Task<List<PostDto>> GetLatestAsync(Guid meId, int size)
    {
        var posts = await _posts.GetLatestAsync(Clamp(size));
        return await BuildListAsync(posts, meId);
    }

    public async Task<List<PostDto>> GetMyPostsAsync(Guid meId, int size)
    {
        var posts = await _posts.GetByAuthorAsync(meId, Clamp(size));
        return await BuildListAsync(posts, meId);
    }

    private async Task<List<PostDto>> BuildListAsync(List<Post> posts, Guid meId)
    {
        var list = new List<PostDto>(posts.Count);
        foreach (var p in posts)
            list.Add(await BuildDtoAsync(p, meId));
        return list;
    }
    public async Task<List<PostDto>> GetByUserAsync(Guid viewerId, Guid userId, int size)
    {
        var posts = await _posts.GetByAuthorAsync(userId, Clamp(size));
        return await BuildListAsync(posts, viewerId);
    }

    private async Task<PostDto> BuildDtoAsync(Post p, Guid meId)
    {
        var urls = await _media.GetUrlsByPostAsync(p.Id);
        var commentCount = await _comments.CountByPostAsync(p.Id);

        var my = await _reactions.GetMineAsync(p.Id, meId);

        var counts = (await _reactions.GetCountsAsync(p.Id))
            .Select(x => new ReactionCountDto(x.Type, x.Count))
            .ToList();

        return new PostDto(
            p.Id,
            p.AuthorId,
            p.AuthorUserName,
            p.Content,
            p.Privacy,
            p.CreatedAt,
            commentCount,
            urls,
            my?.Type,
            counts
        );
    }
    public async Task<PostDto> UpdatePostAsync(Guid postId, UpdatePostReq req, Guid userId)
    {
        var post = await _posts.GetAsync(postId) ?? throw new Exception("Post not found");

        if (post.AuthorId != userId)
            throw new Exception("You can only edit your own post");

        post.Content = req.Content ?? "";
        post.Privacy = string.IsNullOrWhiteSpace(req.Privacy) ? post.Privacy : req.Privacy;

        await _posts.UpdateAsync(post);

        // trả dto mới
        return await BuildDtoAsync(post, userId);
    }

    public async Task<object> DeletePostAsync(Guid postId, Guid userId)
    {
        var post = await _posts.GetAsync(postId) ?? throw new Exception("Post not found");

        if (post.AuthorId != userId)
            throw new Exception("You can only delete your own post");

        // Xóa “con”
        await _comments.DeleteByPostAsync(postId);
        await _likes.RemoveAllByPostAsync(postId);

        // nếu có reactions
        // await _reactions.RemoveAllByPostAsync(postId);

        await _media.DeleteByPostAsync(postId);

        // Xóa post
        await _posts.DeleteAsync(post);

        return new { ok = true };
    }

    public async Task<PostCommentDto> UpdateCommentAsync(Guid commentId, UpdateCommentReq req, Guid userId)
    {
        var cmt = await _comments.GetAsync(commentId) ?? throw new Exception("Comment not found");

        if (cmt.UserId != userId)
            throw new Exception("You can only edit your own comment");

        cmt.Content = req.Content ?? "";
        await _comments.UpdateAsync(cmt);

        return new PostCommentDto(cmt.Id, cmt.PostId, cmt.UserId, cmt.UserName, cmt.Content, cmt.CreatedAt);
    }

    public async Task<object> DeleteCommentAsync(Guid commentId, Guid userId)
    {
        var cmt = await _comments.GetAsync(commentId) ?? throw new Exception("Comment not found");

        if (cmt.UserId != userId)
            throw new Exception("You can only delete your own comment");

        await _comments.DeleteAsync(cmt);
        return new { ok = true };
    }
}
