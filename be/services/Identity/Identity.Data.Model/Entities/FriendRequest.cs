namespace Identity.Data.Model.Entities;

public class FriendRequest
{
    public Guid Id { get; set; }
    public Guid FromUserId { get; set; }
    public Guid ToUserId { get; set; }
    public byte Status { get; set; } // 0=pending, 1=accepted
    public DateTime CreatedAt { get; set; }
}
