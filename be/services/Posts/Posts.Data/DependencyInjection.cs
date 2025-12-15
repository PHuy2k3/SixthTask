using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Posts.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddPostsData(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<PostsDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("PostsDb")));

        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
