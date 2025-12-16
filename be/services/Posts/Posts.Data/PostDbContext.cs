using Microsoft.EntityFrameworkCore;
using Posts.Data.Model.Entities;

namespace Posts.Data;

public class PostsDbContext : DbContext
{
    public PostsDbContext(DbContextOptions<PostsDbContext> options) : base(options) { }

    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PostLike> Likes => Set<PostLike>();
    public DbSet<PostComment> Comments => Set<PostComment>();
    public DbSet<PostMedia> PostMedia => Set<PostMedia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>().HasKey(x => x.Id);
        modelBuilder.Entity<PostLike>().HasKey(x => new { x.PostId, x.UserId });
        modelBuilder.Entity<PostComment>().HasKey(x => x.Id);
        modelBuilder.Entity<PostComment>().HasIndex(x => x.PostId);
        modelBuilder.Entity<PostMedia>().HasKey(x => x.Id);
        modelBuilder.Entity<PostMedia>().HasIndex(x => x.PostId);
    }
}
