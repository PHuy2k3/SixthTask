using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Notifications.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationsData(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<NotificationsDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("NotificationsDb")));

        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
