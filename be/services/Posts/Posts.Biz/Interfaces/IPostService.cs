using Posts.Biz.Model;

namespace Posts.Biz.Interfaces;

public interface IPostService
{
    Task<PostDto> CreateAsync(CreatePostReq req, Guid userId, string userName, List<string>? imageUrls = null);

    Task<List<PostDto>> GetLatestAsync(Guid meId, int size);
    Task<List<PostDto>> GetMyPostsAsync(Guid meId, int size);

    // ✅ COMMENT
    Task<PostCommentDto> AddCommentAsync(Guid postId, AddCommentReq req, Guid userId, string userName);
    Task<List<PostCommentDto>> GetCommentsAsync(Guid postId);

    // ✅ REACTION
    Task<PostDto> SetReactionAsync(Guid postId, byte type, Guid userId, string userName);
    Task<List<PostDto>> GetByUserAsync(Guid viewerId, Guid userId, int size);

}
