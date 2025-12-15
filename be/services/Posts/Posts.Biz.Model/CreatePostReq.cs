namespace Posts.Biz.Model;

public sealed record CreatePostReq(string Content, string Privacy);
public sealed record PostDto(Guid Id, Guid AuthorId, string Content, string Privacy, DateTime CreatedAt);
