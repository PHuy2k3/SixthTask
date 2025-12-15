using Posts.Data.Model.Entities;

namespace Posts.Data;

public interface IPostRepository
{
    Task AddAsync(Post post);
    Task<List<Post>> GetLatestAsync(int size);
}
