namespace Notifications.Data.Model.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public Guid RecipientUserId { get; set; }
    public Guid ActorUserId { get; set; }
    public string ActorUserName { get; set; } = "";
    public string Type { get; set; } = ""; // like | comment
    public Guid PostId { get; set; }
    public string? Content { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
