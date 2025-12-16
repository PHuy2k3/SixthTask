namespace Posts.Data.Model.Entities;

public class PostMedia
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string Url { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
