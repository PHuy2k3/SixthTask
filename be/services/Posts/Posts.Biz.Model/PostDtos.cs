namespace Posts.Biz.Model;

public class CreatePostReq
{
    public string Content { get; set; } = "";
    public string Privacy { get; set; } = "public";
}

public class AddCommentReq
{
    public string Content { get; set; } = "";
}
public sealed record PostCommentDto(
    Guid Id,
    Guid PostId,
    Guid UserId,
    string UserName,
    string Content,
    DateTime CreatedAt
);

public record PostDto(
    Guid Id,
    Guid AuthorId,
    string AuthorUserName,
    string Content,
    string Privacy,
    DateTime CreatedAt,
    int LikeCount,
    int CommentCount,
    bool LikedByMe,
    List<string> ImageUrls
);

