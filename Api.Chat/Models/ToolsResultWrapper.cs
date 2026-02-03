using LLM.Abstractions.Models;

namespace Api.Chat.Models;

public class ToolsResultWrapper
{
    public List<Tool> Tools { get; set; }
}