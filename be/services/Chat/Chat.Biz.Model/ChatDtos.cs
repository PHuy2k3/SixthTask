namespace Chat.Biz.Model;

public sealed record UserLiteDto(Guid Id, string UserName);

public sealed record ConversationDto(
    Guid Id,
    UserLiteDto OtherUser,
    string? LastMessage,
    DateTime? LastMessageAt,
    int UnreadCount
);

public sealed record MessageDto(
    Guid Id,
    Guid ConversationId,
    Guid SenderId,
    string SenderUserName,
    string Content,
    DateTime CreatedAt,
    bool IsRead
);

public sealed class CreateConversationReq
{
    public Guid OtherUserId { get; set; }
    public string OtherUserName { get; set; } = "";
}

public sealed class SendMessageReq
{
    public Guid ConversationId { get; set; }
    public string Content { get; set; } = "";
}

public sealed class MarkReadReq
{
    public Guid ConversationId { get; set; }
}
