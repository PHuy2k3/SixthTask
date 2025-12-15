using Microsoft.Extensions.DependencyInjection;

namespace Posts.Biz;

public static class DependencyInjection
{
    public static IServiceCollection AddPostsBiz(this IServiceCollection services)
    {
        services.AddScoped<IPostService, PostService>();
        return services;
    }
}
