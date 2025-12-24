namespace Chat.Data.Model.Entities;

public class Message
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }

    public Guid SenderId { get; set; }
    public string SenderUserName { get; set; } = "";

    public string Content { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsRead { get; set; } = false;

    public Conversation? Conversation { get; set; }
}
