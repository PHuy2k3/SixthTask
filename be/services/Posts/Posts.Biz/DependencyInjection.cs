using Microsoft.Extensions.DependencyInjection;
using Posts.Biz.Interfaces;
using Posts.Biz.Services;

namespace Posts.Biz;

public static class DependencyInjection
{
    public static IServiceCollection AddPostsBiz(this IServiceCollection services)
    {
        IServiceCollection serviceCollection = services.AddScoped<IPostService, PostService>();
        return services;
    }
}
