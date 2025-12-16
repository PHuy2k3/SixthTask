using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Posts.Data.Model.Entities;

namespace Posts.Data.Model.Mapping;

public class PostCommentConfiguration : IEntityTypeConfiguration<PostComment>
{
    public void Configure(EntityTypeBuilder<PostComment> e)
    {
        e.ToTable("PostComments");
        e.HasKey(x => x.Id);

        e.Property(x => x.UserName).IsRequired().HasMaxLength(100);
        e.Property(x => x.Content).IsRequired().HasMaxLength(2000);
        e.Property(x => x.CreatedAt).IsRequired();

        e.HasIndex(x => x.PostId);
    }
}
