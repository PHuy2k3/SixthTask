using Chat.Biz.Interfaces;
using Chat.Biz.Model;
using Chat.Data.Interfaces;
using Chat.Data.Model.Entities;

namespace Chat.Biz;

public class ChatService(
    IConversationRepository convRepo,
    IMessageRepository msgRepo
) : IChatService
{
    static (Guid min, Guid max) Pair(Guid a, Guid b)
        => a.CompareTo(b) <= 0 ? (a, b) : (b, a);

    static UserLiteDto OtherOf(Conversation c, Guid meId)
        => c.UserAId == meId
            ? new UserLiteDto(c.UserBId, c.UserBName)
            : new UserLiteDto(c.UserAId, c.UserAName);

    public async Task<ConversationDto> CreateOrGetConversationAsync(Guid meId, string meName, Guid otherId, string otherName)
    {
        if (meId == otherId) throw new Exception("Cannot chat with yourself");

        var (min, max) = Pair(meId, otherId);

        var existing = await convRepo.FindByPairAsync(min, max);
        if (existing != null)
        {
            // đảm bảo tên được update (phòng trường hợp đổi username)
            if (existing.UserAId == meId && existing.UserAName != meName) existing.UserAName = meName;
            if (existing.UserBId == meId && existing.UserBName != meName) existing.UserBName = meName;

            if (existing.UserAId == otherId && existing.UserAName != otherName) existing.UserAName = otherName;
            if (existing.UserBId == otherId && existing.UserBName != otherName) existing.UserBName = otherName;

            await convRepo.SaveAsync();

            var unread = await msgRepo.CountUnreadAsync(existing.Id, meId);
            return new ConversationDto(existing.Id, OtherOf(existing, meId), existing.LastMessage, existing.LastMessageAt, unread);
        }

        var c = new Conversation
        {
            Id = Guid.NewGuid(),
            UserAId = meId,
            UserAName = meName,
            UserBId = otherId,
            UserBName = otherName,
            UserMin = min,
            UserMax = max,
            CreatedAt = DateTime.UtcNow
        };

        await convRepo.AddAsync(c);
        await convRepo.SaveAsync();

        return new ConversationDto(c.Id, OtherOf(c, meId), null, null, 0);
    }

    public async Task<List<ConversationDto>> GetMyConversationsAsync(Guid meId, int size)
    {
        var list = await convRepo.GetMineAsync(meId, size);
        var result = new List<ConversationDto>(list.Count);

        foreach (var c in list)
        {
            var unread = await msgRepo.CountUnreadAsync(c.Id, meId);
            result.Add(new ConversationDto(c.Id, OtherOf(c, meId), c.LastMessage, c.LastMessageAt, unread));
        }

        return result;
    }

    public async Task<List<MessageDto>> GetMessagesAsync(Guid meId, Guid conversationId, int size)
    {
        var c = await convRepo.GetAsync(conversationId) ?? throw new Exception("Conversation not found");
        if (c.UserAId != meId && c.UserBId != meId) throw new Exception("Forbidden");

        var msgs = await msgRepo.GetByConversationAsync(conversationId, size);
        return msgs.Select(m => new MessageDto(m.Id, m.ConversationId, m.SenderId, m.SenderUserName, m.Content, m.CreatedAt, m.IsRead)).ToList();
    }

    public async Task<MessageDto> SendMessageAsync(Guid meId, string meName, Guid conversationId, string content)
    {
        content = content?.Trim() ?? "";
        if (content.Length == 0) throw new Exception("Message content is required");

        var conv = await convRepo.GetTrackAsync(conversationId) ?? throw new Exception("Conversation not found");
        if (conv.UserAId != meId && conv.UserBId != meId) throw new Exception("Forbidden");

        var m = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderId = meId,
            SenderUserName = meName,
            Content = content,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await msgRepo.AddAsync(m);

        conv.LastMessage = content.Length > 120 ? content[..120] : content;
        conv.LastMessageAt = m.CreatedAt;

        await convRepo.SaveAsync(); // save cả message + update conv

        return new MessageDto(m.Id, m.ConversationId, m.SenderId, m.SenderUserName, m.Content, m.CreatedAt, m.IsRead);
    }


    public async Task MarkReadAsync(Guid meId, Guid conversationId)
    {
        var conv = await convRepo.GetAsync(conversationId) ?? throw new Exception("Conversation not found");
        if (conv.UserAId != meId && conv.UserBId != meId) throw new Exception("Forbidden");

        await msgRepo.MarkReadAsync(conversationId, meId);
    }
}
