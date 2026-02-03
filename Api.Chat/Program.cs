using Api.Chat.Providers;
using Chat.ServerLibrary;
using Chat.ServerLibrary.Controllers;
using LLM.Abstractions.Interfaces;
using LLM.OpenAIClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddChatServices();
builder.Services.AddOpenAIProvider(options =>
{
    options.Model = "openai/gpt-oss-120b";
    options.BaseUrl = "https://api.groq.com/openai/v1/";
    options.RelativeEndpoint = "chat/completions";
    options.AuthenticationHeaderValue = "Bearer ";
    options.Temperature = 1;
    options.MaxCompletionTokens = 8192;
});
builder.Services.AddSingleton<IToolProvider, BlazzingPizzaToolProvider>();
builder.Services.AddCors(builder =>
{
    builder.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseChatEndpoints();
app.UseCors();
app.Run();

