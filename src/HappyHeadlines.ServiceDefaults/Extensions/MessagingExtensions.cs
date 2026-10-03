using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HappyHeadlines.ServiceDefaults.Extensions;

public static class MessagingExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Messaging") ??
                               throw new InvalidOperationException("Missing ConnectionStrings:Messaging");
        services.AddEasyNetQ(connectionString);
        services.AddSingleton<IMessageClient, MessageClient>();

        return services;
    }
}
