using Identity.Data.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Data.Model.Mapping;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> e)
    {
        e.ToTable("RefreshTokens");
        e.HasKey(x => x.Id);

        e.Property(x => x.TokenHash).IsRequired().HasMaxLength(500);
        e.Property(x => x.ExpiresAt).IsRequired();
        e.Property(x => x.CreatedAt).IsRequired();
        e.Property(x => x.IsRevoked).IsRequired();

        e.HasIndex(x => x.UserId);
        e.HasIndex(x => x.TokenHash);

        e.HasOne(x => x.User)
         .WithMany(u => u.RefreshTokens)
         .HasForeignKey(x => x.UserId);
    }
}
