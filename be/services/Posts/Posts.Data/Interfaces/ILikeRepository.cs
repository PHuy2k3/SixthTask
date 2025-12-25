using Posts.Data.Model.Entities;

namespace Posts.Data.Interfaces;

public interface ILikeRepository
{
    Task AddAsync(PostLike like);
    Task RemoveAsync(Guid postId, Guid userId);
    Task<bool> ExistsAsync(Guid postId, Guid userId);
    Task<int> CountByPostAsync(Guid postId);
    Task RemoveAllByPostAsync(Guid postId);

}
