using Chat.Proxies;
using Chat.ViewModels;
using DemoChatLLM.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddChatViewModels();
builder.Services.AddChatProxies(options =>
{
    options.BaseUrl = builder.Configuration["ChatApiBaseUrl"];
    options.RelativeUrl = builder.Configuration["ChatApiRelativeUrl"];
});

await builder.Build().RunAsync();
