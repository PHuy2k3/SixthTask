using Chat.Biz.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Biz;

public static class ChatBizDI
{
    public static IServiceCollection AddChatBiz(this IServiceCollection services)
    {
        services.AddScoped<IChatService, ChatService>();
        return services;
    }
}
