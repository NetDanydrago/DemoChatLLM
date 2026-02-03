using System.ComponentModel.DataAnnotations;

namespace Chat.ViewModels.Models;

public class ChatMessageModel
{
    public string Role { get; set; }
    
    [Required(ErrorMessage = "El mensaje no puede estar vacío")]
    public string Content { get; set; }
    
    public DateTime Timestamp { get; set; }
    
    public bool IsUser => Role == "user";
}
