using Posts.Data.Model.Entities;

namespace Posts.Data.Interfaces;

public interface ICommentRepository
{
    Task AddAsync(PostComment cmt);
    Task<int> CountByPostAsync(Guid postId);
    Task<List<PostComment>> GetByPostAsync(Guid postId);
}
