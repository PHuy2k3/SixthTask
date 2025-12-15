using Identity.Biz.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Biz;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityBiz(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<JwtIssuer>();
        return services;
    }
}
