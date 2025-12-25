using Posts.Data.Model.Entities;

namespace Posts.Data.Interfaces;

public interface IPostMediaRepository
{
    Task AddManyAsync(List<PostMedia> items);
    Task<List<string>> GetUrlsByPostAsync(Guid postId);
    Task DeleteByPostAsync(Guid postId);

}
