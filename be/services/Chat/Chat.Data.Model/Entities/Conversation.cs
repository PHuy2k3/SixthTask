namespace Chat.Data.Model.Entities;

public class Conversation
{
    public Guid Id { get; set; }

    // 2 người tham gia
    public Guid UserAId { get; set; }
    public string UserAName { get; set; } = "";

    public Guid UserBId { get; set; }
    public string UserBName { get; set; } = "";

    // để unique theo cặp người dùng (tránh CASE lỗi)
    public Guid UserMin { get; set; }
    public Guid UserMax { get; set; }

    public string? LastMessage { get; set; }
    public DateTime? LastMessageAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Message> Messages { get; set; } = new();
}
