using Identity.Data.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Data.Model.Mapping;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> e)
    {
        e.ToTable("Users");
        e.HasKey(x => x.Id);

        e.Property(x => x.UserName).IsRequired().HasMaxLength(100);
        e.Property(x => x.Email).IsRequired().HasMaxLength(200);
        e.Property(x => x.PasswordHash).IsRequired().HasMaxLength(500);
        e.Property(x => x.CreatedAt).IsRequired();
        e.Property(x => x.IsActive).IsRequired();

        e.HasIndex(x => x.UserName).IsUnique();
        e.HasIndex(x => x.Email).IsUnique();
    }
}
