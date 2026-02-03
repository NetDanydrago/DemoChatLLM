using Chat.ViewModels.Models;

namespace Chat.ViewModels.Abstractions;

public interface IChatViewModel
{
    List<ChatMessageModel> Messages { get; }
    bool IsLoading { get; }
    bool HasError { get; }
    string ErrorMessage { get; }
    
    Task SendMessageAsync(string message);
    void ClearMessages();
}
