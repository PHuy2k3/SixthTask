using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Posts.Data.Model.Entities;

namespace Posts.Data.Model.Mapping;

public class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> e)
    {
        e.ToTable("PostLikes");
        e.HasKey(x => new { x.PostId, x.UserId });
        e.Property(x => x.CreatedAt).IsRequired();
        e.HasIndex(x => x.UserId);
    }
}
