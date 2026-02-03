using Chat.ViewModels.Abstractions;
using Chat.ViewModels.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.ViewModels;

public static class DependencyContainer
{
    public static IServiceCollection AddChatViewModels(this IServiceCollection services)
    {
        services.AddScoped<IChatViewModel, ChatViewModel>();
        
        return services;
    }
}
