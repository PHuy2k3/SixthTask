using Identity.Data.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Data;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> opt) : base(opt) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<FriendRequest> FriendRequests => Set<FriendRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(User).Assembly);
        modelBuilder.Entity<FriendRequest>(e =>
        {
            e.ToTable("FriendRequests");
            e.HasKey(x => x.Id);

            e.Property(x => x.Status).IsRequired();
            e.Property(x => x.CreatedAt).IsRequired();

            e.HasIndex(x => new { x.FromUserId, x.ToUserId }).IsUnique();
            e.HasIndex(x => x.ToUserId);
        });
    }
}
