namespace Posts.Data.Model.Entities;

public class PostLike
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
