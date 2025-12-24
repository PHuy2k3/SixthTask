using Chat.Data.Interfaces;
using Chat.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Data;

public static class ChatDataDI
{
    public static IServiceCollection AddChatData(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<ChatDbContext>(opt =>
            opt.UseSqlServer(cfg.GetConnectionString("ChatDb")));

        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        return services;
    }
}
