using Chat.Proxies.Abstractions;
using LLM.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chat.Proxies;

internal class ChatProxy(IChatClient chatClient, ILogger<ChatProxy> logger) : IChatProxy
{
    public async Task<string> SendMessageAsync(string message)
    {
        string result;
        try
        {
            var response = await chatClient.ChatAsync(message);

            if (response.IsSuccess)
            {
                result = response.Value;
            }
            else
            {
                logger.LogError("Error en la respuesta del chat: {Error}", response.Error);
                result = "Error al obtener respuesta del asistente.";
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al enviar mensaje al chat");
            throw;
        }

        return result;
    }
}
