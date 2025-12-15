using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityData(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<IdentityDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("IdentityDb")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
