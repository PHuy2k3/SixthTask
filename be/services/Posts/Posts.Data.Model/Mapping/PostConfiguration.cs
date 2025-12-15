using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Posts.Data.Model.Entities;

namespace Posts.Data.Model.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> e)
    {
        e.ToTable("Posts");

        e.HasKey(x => x.Id);

        e.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(5000);

        e.Property(x => x.Privacy)
            .IsRequired()
            .HasMaxLength(20);

        e.Property(x => x.CreatedAt)
            .IsRequired();

        e.HasIndex(x => x.AuthorId);
        e.HasIndex(x => x.CreatedAt);
    }
}
