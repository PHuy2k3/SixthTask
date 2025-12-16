using Posts.Biz.Model;

namespace Posts.Biz.Interfaces;

public interface IPostService
{
    Task<PostDto> CreateAsync(CreatePostReq req, Guid userId, string userName, List<string>? imageUrls = null);
    Task<PostDto> ToggleLikeAsync(Guid postId, Guid userId);
    Task<PostCommentDto> AddCommentAsync(Guid postId, AddCommentReq req, Guid userId, string userName);
    Task<List<PostDto>> GetLatestAsync(Guid meId, int size);
    Task<List<PostDto>> GetMyPostsAsync(Guid meId, int size);
    Task<List<PostCommentDto>> GetCommentsAsync(Guid postId);
}
