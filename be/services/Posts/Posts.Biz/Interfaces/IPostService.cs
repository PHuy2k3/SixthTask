using Posts.Biz.Model;

namespace Posts.Biz;

public interface IPostService
{
    Task<PostDto> CreateAsync(CreatePostReq req, Guid userId);
    Task<List<PostDto>> GetLatestAsync(int size);
}
