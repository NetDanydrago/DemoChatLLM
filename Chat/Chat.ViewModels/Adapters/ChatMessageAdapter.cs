using Chat.ViewModels.Models;
using Entities.Dtos;

namespace Chat.ViewModels.Adapters;

internal static class ChatMessageAdapter
{
    public static ChatMessageModel ToChatMessageModel(this ChatMessageDto dto)
    {
        return new ChatMessageModel
        {
            Role = dto.Role,
            Content = dto.Content,
            Timestamp = dto.Timestamp
        };
    }

    public static List<ChatMessageModel> ToChatMessageModels(this IEnumerable<ChatMessageDto> dtos)
    {
        return dtos.Select(dto => dto.ToChatMessageModel()).ToList();
    }
    
    public static ChatMessageDto ToChatMessageDto(this ChatMessageModel model)
    {
        return new ChatMessageDto
        {
            Role = model.Role,
            Content = model.Content,
            Timestamp = model.Timestamp
        };
    }
}
