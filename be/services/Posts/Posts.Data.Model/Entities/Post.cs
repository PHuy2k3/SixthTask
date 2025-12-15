namespace Posts.Data.Model.Entities;

public class Post
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = "";
    public string Privacy { get; set; } = "public";
    public DateTime CreatedAt { get; set; }
}
