namespace Chat.Proxies.Abstractions;

public interface IChatProxy
{
    Task<string> SendMessageAsync(string message);
}
