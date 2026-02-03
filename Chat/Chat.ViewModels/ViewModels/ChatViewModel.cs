using Chat.Proxies.Abstractions;
using Chat.ViewModels.Abstractions;
using Chat.ViewModels.Models;
using Microsoft.Extensions.Logging;

namespace Chat.ViewModels.ViewModels;

public class ChatViewModel(IChatProxy chatProxy, ILogger<ChatViewModel> logger) : IChatViewModel
{
    public List<ChatMessageModel> Messages { get; } = [];
    public bool IsLoading { get; private set; }
    public bool HasError { get; private set; }
    public string ErrorMessage { get; private set; }

    public async Task SendMessageAsync(string message)
    {
        try
        {
            var userMessage = new ChatMessageModel
            {
                Role = "user",
                Content = message,
                Timestamp = DateTime.Now
            };        
            Messages.Add(userMessage);
            IsLoading = true;
            var response = await chatProxy.SendMessageAsync(message);
            var assistantMessage = new ChatMessageModel
            {
                Role = "assistant",
                Content = response,
                Timestamp = DateTime.Now
            };
            Messages.Add(assistantMessage);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error al enviar mensaje. Por favor, intenta nuevamente.";
            HasError = true;
        }
        IsLoading = false;
  
    }

    public void ClearMessages()
    {
        HasError = false;
        ErrorMessage = string.Empty;
        Messages.Clear();
    }

}
