using Microsoft.EntityFrameworkCore;
using Notifications.Data.Model.Entities;

namespace Notifications.Data;

public class NotificationsDbContext : DbContext
{
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> opt) : base(opt) { }

    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Notification).Assembly);
    }
}
