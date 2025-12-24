namespace Posts.Data.Model.Entities;

public class PostReaction
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public byte Type { get; set; } // 0=Like,1=Love,2=Haha,3=Wow,4=Sad,5=Angry
    public DateTime CreatedAt { get; set; }
}
