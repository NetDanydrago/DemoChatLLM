# 🚀 DemoChatLLM - Cliente Blazor con Chat LLM

Demo presentado en **NetConf 2025 Puebla** que muestra la integración de un cliente Blazor WebAssembly con una API ASP.NET Core que expone capacidades de un Large Language Model (LLM) con soporte para ejecución de herramientas.

## 📋 Descripción

Este proyecto demuestra una arquitectura completa de chat impulsado por IA:

- **Cliente:** Aplicación Blazor WebAssembly (DemoChatLLM.Web) con interfaz de chat interactiva
- **API:** ASP.NET Core Web API (Api.Chat) que sirve como puente hacia servicios LLM (OpenAI, Groq, etc.)
- **Arquitectura:** Implementación de vertical slice con separación en capas (Proxies, ViewModels, Views)

## 🎓 Basado en el Curso MCP

Los paquetes NuGet locales utilizados en este proyecto fueron desarrollados en base al curso:

**"MCP para Programadores de C#"**  
🔗 [https://devscommunity.net/mcp/](https://devscommunity.net/mcp/)

Este curso enseña cómo implementar el Model Context Protocol (MCP) en aplicaciones C# para integrar capacidades avanzadas de IA.

## 🏗️ Arquitectura del Proyecto

```
DemoChatLLM.sln
├── Api.Chat/                      # API Backend con endpoints LLM
│   ├── Program.cs
│   ├── appsettings.json
│   ├── Models/
│   │   ├── McpResponseDto.cs
│   │   └── ToolsResultWrapper.cs
│   └── Providers/
│       └── BlazzingPizzaToolProvider.cs
│
├── DemoChatLLM.Web/               # Cliente Blazor WebAssembly
│   ├── Program.cs
│   ├── wwwroot/
│   │   └── appsettings.json
│   └── Shared/
│       ├── MainLayout.razor
│       └── NavMenu.razor
│
├── Chat/                          # Módulo Chat (Vertical Slice)
│   ├── Chat.Proxies/              # Comunicación con la API
│   ├── Chat.ViewModels/           # Lógica de presentación
│   └── Chat.Views/                # Componentes UI
│
├── Common/
│   └── Entities/                  # DTOs compartidos
│       └── Dtos/
│           └── ChatMessageDto.cs
│
└── LocalNugets/                   # Paquetes locales del curso MCP
```

## ⚙️ Configuración

### Prerrequisitos

- **.NET 10.0 SDK** (o superior)
- **Visual Studio 2022** o **VS Code** con extensión C#
- **Clave API** de un proveedor LLM (OpenAI, Groq, etc.)

### Paso 1: Configurar la API Backend

1. **Abrir** [Api.Chat/appsettings.json](Api.Chat/appsettings.json)

2. **No es necesario modificar nada** - la configuración está en el código:

```csharp
builder.Services.AddOpenAIProvider(options =>
{
    options.Model = "openai/gpt-oss-120b";
    options.BaseUrl = "https://api.groq.com/openai/v1/";
    options.RelativeEndpoint = "chat/completions";
    options.AuthenticationHeaderValue = "Bearer TU_API_KEY_AQUI";
    options.Temperature = 1;
    options.MaxCompletionTokens = 8192;
});
```

3. **Reemplazar** `TU_API_KEY_AQUI` en [Api.Chat/Program.cs](Api.Chat/Program.cs#L18) con tu clave API:

   - Para **Groq:** Obtén tu clave en [https://console.groq.com/keys](https://console.groq.com/keys)
   - Para **OpenAI:** Usa `https://api.openai.com/v1/` como `BaseUrl` y tu clave de OpenAI

### Paso 2: Configurar el Cliente Blazor

1. **Abrir** [DemoChatLLM.Web/wwwroot/appsettings.json](DemoChatLLM.Web/wwwroot/appsettings.json)

2. **Verificar** que la URL de la API coincida con el puerto configurado en la API:

```json
{
  "ChatApiBaseUrl": "https://localhost:7135/",
  "ChatApiRelativeUrl": "chat"
}
```

3. Si el puerto de la API es diferente, actualizar `ChatApiBaseUrl` según [Api.Chat/Properties/launchSettings.json](Api.Chat/Properties/launchSettings.json)

### Paso 3: Restaurar Paquetes

Los paquetes locales del curso MCP están en la carpeta `LocalNugets/`. Para restaurarlos:

```bash
dotnet restore
```

## 🚀 Ejecución

### Opción 1: Proyectos Múltiples en Visual Studio

1. **Clic derecho** en la solución `DemoChatLLM.sln`
2. Seleccionar **"Configurar proyectos de inicio"**
3. Elegir **"Proyectos de inicio múltiples"**
4. Configurar:
   - `Api.Chat` → **Iniciar**
   - `DemoChatLLM.Web` → **Iniciar**
5. Presionar **F5** o hacer clic en **Iniciar**

### Opción 2: Línea de Comandos

**Terminal 1 - API:**
```bash
cd Api.Chat
dotnet run
```

**Terminal 2 - Cliente Blazor:**
```bash
cd DemoChatLLM.Web
dotnet run
```

### Verificación

- **API:** [https://localhost:7135/openapi](https://localhost:7135/openapi)
- **Cliente:** [https://localhost:7036](https://localhost:7036) (o el puerto que asigne Kestrel)

## 🎯 Características

### Cliente Blazor (DemoChatLLM.Web)
- ✅ Interfaz de chat responsive (mobile-first)
- ✅ Comunicación en tiempo real con la API
- ✅ Gestión de estado con ViewModels
- ✅ Inyección de dependencias centralizada

### API Backend (Api.Chat)
- ✅ Endpoints para chat con LLM
- ✅ Soporte para ejecución de herramientas (MCP)
- ✅ Integración con OpenAI/Groq compatible APIs
- ✅ CORS configurado para desarrollo
- ✅ Provider de ejemplo: BlazzingPizzaToolProvider

## 🛠️ Tecnologías Utilizadas

- **Frontend:** Blazor WebAssembly (.NET 10)
- **Backend:** ASP.NET Core Web API (.NET 10)
- **LLM Client:** Implementación custom compatible con OpenAI
- **MCP:** Model Context Protocol para ejecución de herramientas
- **Arquitectura:** Vertical Slice con Primary Constructors

## 📚 Estructura de Capas

Cada módulo (ej: `Chat/`) sigue una arquitectura de 3 capas:

1. **Proxies:** Comunicación HTTP con la API
   - Interfaces (`IChatProxy`)
   - Implementaciones (`ChatProxy`)
   - DI Container

2. **ViewModels:** Lógica de presentación y estado
   - Interfaces (`IChatViewModel`)
   - Modelos (`ChatMessageModel`)
   - Adaptadores (`ChatMessageAdapter`)
   - ViewModels (`ChatViewModel`)

3. **Views:** Componentes Blazor
   - Páginas Razor (`ChatPage.razor`)
   - Code-behind (`ChatPage.razor.cs`)

## 🔧 Solución de Problemas

### Error de CORS
Si ves errores de CORS en la consola del navegador:
- Verificar que la API esté ejecutándose
- Confirmar que `app.UseCors()` está configurado en [Api.Chat/Program.cs](Api.Chat/Program.cs#L40)

### Error de Conexión
Si el cliente no se conecta a la API:
- Revisar los puertos en `launchSettings.json` de ambos proyectos
- Actualizar `ChatApiBaseUrl` en el cliente según el puerto de la API

### Paquetes NuGet Locales no Encontrados
Si faltan referencias a paquetes del curso MCP:
```bash
dotnet nuget add source ./LocalNugets --name LocalMCP
dotnet restore
```


## 📄 Licencia

Este proyecto es un demo educativo presentado en el NetConf 2025 Puebla.

## 🔗 Enlaces Relacionados

- **Curso MCP para C#:** [https://devscommunity.net/mcp/](https://devscommunity.net/mcp/)
- **NetConf 2025 Puebla:** [Evento oficial]
- **Groq Console:** [https://console.groq.com/](https://console.groq.com/)
- **OpenAI Platform:** [https://platform.openai.com/](https://platform.openai.com/)

---

**¡Desarrollado para NetConf 2025 Puebla! 🇲🇽**
