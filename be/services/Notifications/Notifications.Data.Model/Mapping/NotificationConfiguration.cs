using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notifications.Data.Model.Entities;

namespace Notifications.Data.Model.Mapping;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> e)
    {
        e.ToTable("Notifications");
        e.HasKey(x => x.Id);

        e.Property(x => x.ActorUserName).IsRequired().HasMaxLength(100);
        e.Property(x => x.Type).IsRequired().HasMaxLength(20);
        e.Property(x => x.Content).HasMaxLength(2000);
        e.Property(x => x.CreatedAt).IsRequired();

        e.HasIndex(x => x.RecipientUserId);
    }
}
