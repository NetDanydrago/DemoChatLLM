using Chat.ViewModels.Abstractions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Chat.Views.Pages;

public partial class ChatPage(IChatViewModel viewModel, IJSRuntime jsRuntime)
{
    private IChatViewModel ViewModel { get; } = viewModel;
    private string MessageInput { get; set; }

    private async Task SendMessage()
    {
        if (!string.IsNullOrWhiteSpace(MessageInput))
        {
            var message = MessageInput;
            MessageInput = string.Empty;
            await ViewModel.SendMessageAsync(message);
        }
    }

    private async Task HandleKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Enter" && !args.ShiftKey)
        {
            await SendMessage();
        }
    }

    private void ClearChat()
    {
        ViewModel.ClearMessages();
    }

}
