using Microsoft.EntityFrameworkCore;
using Posts.Data.Model;
using Posts.Data.Model.Entities;

namespace Posts.Data;

public class PostsDbContext : DbContext
{
    public PostsDbContext(DbContextOptions<PostsDbContext> opt) : base(opt) { }

    public DbSet<Post> Posts => Set<Post>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(Post).Assembly
        );
    }
}
