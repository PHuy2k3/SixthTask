using Chat.Data.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chat.Data;

public class ChatDbContext : DbContext
{
    public ChatDbContext(DbContextOptions<ChatDbContext> opt) : base(opt) { }

    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Conversation>(e =>
        {
            e.ToTable("Conversations");
            e.HasKey(x => x.Id);

            e.Property(x => x.UserAName).HasMaxLength(100);
            e.Property(x => x.UserBName).HasMaxLength(100);

            e.Property(x => x.LastMessage).HasMaxLength(500);

            e.HasIndex(x => new { x.UserMin, x.UserMax }).IsUnique();
        });

        b.Entity<Message>(e =>
        {
            e.ToTable("Messages");
            e.HasKey(x => x.Id);

            e.Property(x => x.SenderUserName).HasMaxLength(100);
            e.Property(x => x.Content).HasMaxLength(2000);

            e.HasIndex(x => new { x.ConversationId, x.CreatedAt });

            e.HasOne(x => x.Conversation)
             .WithMany(c => c.Messages)
             .HasForeignKey(x => x.ConversationId);
        });
    }
}
