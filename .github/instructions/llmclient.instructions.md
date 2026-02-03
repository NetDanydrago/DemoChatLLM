---
description: Guía para el uso del cliente LLM con capacidades de chat y ejecución de herramientas.
applyTo:  **/*.cs   
---

# Guía de Consumo - Chat LLM

Cómo usar las bibliotecas `LLM.OpenAIWebClient` para crear un chat

---

## Paquetes NuGet Requeridos

```xml
<PackageReference Include="LLM.OpenAIWebClient" />
```

---

## 1. Configuración Básica

```csharp
using LLM.Abstractions;
using LLM.OpenAIClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddChatServices(options => { options.BaseUrl = "https://localhost:7135/"; options.RelativeUrl = "chat"; });

var host = builder.Build();
```

---

## 2. Uso del Chat Client

```csharp
var chatClient = host.Services.GetRequiredService<IChatClient>();

//enviar un mensaje simple
string assistantResponse = string.Empty;
var result = await chatClient.ChatAsync(userMessage);

if (result.IsSuccess)
{
    assistantResponse = result.Value;
}
else
{
    assistantResponse = "Error al obtener respuesta del asistente.";
    logger.LogError("Error en la respuesta del chat: {Error}", result.Error);
}
```

**Por qué es tan simple:** La implementación de `IChatClient` **ya maneja automáticamente el contexto de la conversación**. No necesitas gestionar manualmente el historial de mensajes ni mantener el estado de la conversación. Simplemente envía cada mensaje con `ChatAsync()` y el cliente se encarga del resto.

---



**Nota:** No hardcodear API keys. Usar variables de entorno o configuración segura.
