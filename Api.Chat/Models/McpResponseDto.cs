namespace Api.Chat.Models;

public class McpResponseDto<T>(string jsonRpc, string id, T result)
{
    public string JsonRpc => jsonRpc;
    public string Id => id;
    public T Result => result;
}
