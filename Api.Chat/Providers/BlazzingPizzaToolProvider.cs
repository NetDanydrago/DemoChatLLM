using Api.Chat.Models;
using LLM.Abstractions.Interfaces;
using LLM.Abstractions.Models;
using Results;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api.Chat.Providers;

internal class BlazzingPizzaToolProvider : IToolProvider
{
    private const string McpServerUrl = "https://dynamicqueriesmcpserver.azurewebsites.net/mcp";
    
    private readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<IEnumerable<Tool>> GetToolsAsync()
    {
        var response = await SendJsonRpcRequestAsync("tools/list", new { });
        var result = JsonSerializer.Deserialize<McpResponseDto<ToolsResultWrapper>>(response, JsonOptions);
        return result.Result.Tools;
    }

    public async Task<Result<string>> ExecuteToolAsync(ToolExecuteArguments args)
    {
        var toolParams = new
        {
            name = args.ToolName,
            arguments = args.ToolArguments
        };

        var result = await SendJsonRpcRequestAsync("tools/call", toolParams);
        return Result<string>.Ok(result);
    }

    private async Task<string> SendJsonRpcRequestAsync(string method, object parameters)
    {
        using var httpClient = new HttpClient();
        var request = new
        {
            jsonrpc = "2.0",
            id = Guid.NewGuid().ToString(),
            method = method,
            @params = parameters
        };
        
        var jsonContent = JsonSerializer.Serialize(request, JsonOptions);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(McpServerUrl, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

}
