using Chat.Proxies.Abstractions;
using LLM.OpenAIWebClient;
using LLM.OpenAIWebClient.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Proxies;

public static class DependencyContainer
{
    public static IServiceCollection AddChatProxies(
        this IServiceCollection services,
        Action<ChatClientOptions> configureOptions)
    {
        services.AddChatServices(configureOptions);
        services.AddScoped<IChatProxy, ChatProxy>();

        return services;
    }
}
