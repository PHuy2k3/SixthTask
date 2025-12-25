using Posts.Data.Model.Entities;

namespace Posts.Data.Interfaces;

public interface IPostRepository
{
    Task AddAsync(Post p);
    Task<Post?> GetAsync(Guid id);
    Task<List<Post>> GetLatestAsync(int size);
    Task<List<Post>> GetByAuthorAsync(Guid authorId, int size);
    Task UpdateAsync(Post post);
    Task DeleteAsync(Post post);
}
