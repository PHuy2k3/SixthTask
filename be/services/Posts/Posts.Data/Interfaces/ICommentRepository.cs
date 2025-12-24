using Posts.Data.Model.Entities;

namespace Posts.Data.Interfaces;

public interface ICommentRepository
{
    Task AddAsync(PostComment cmt);
    Task<List<PostComment>> GetByPostAsync(Guid postId);
    Task<int> CountByPostAsync(Guid postId);
}
