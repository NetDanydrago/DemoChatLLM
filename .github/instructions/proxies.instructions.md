---
description: Instrucciones para la creación y uso de proyectos Proxies en la arquitectura del sistema.
applyTo: "**/*.Proxies/**, **/*.Proxies.csproj"
---

## Proyecto Proxies

Los proyectos Proxies son responsables de la **comunicación con servicios externos**. Esto incluye:

1. **Consumo de APIs REST/HTTP** - Comunicación con backend o servicios web
2. **Uso de bibliotecas del proyecto Common** - Reutilización de lógica compartida
3. **Uso de paquetes NuGet** - Que abstraen la lógica de consumo de servicios (ej: SDKs de terceros, clientes de base de datos, servicios cloud, etc.)

**Importante:** Los Proxies encapsulan la lógica de comunicación externa, independientemente de si es HTTP, una biblioteca o un paquete NuGet. El ViewModels NO debe conocer los detalles de implementación, solo debe depender de la interfaz del Proxy.

**Nota:** Las reglas y patrones definidos aquí aplican para todos los tipos de Proxies, ajustando los detalles según el tipo de comunicación (HTTP, biblioteca, SDK, etc.).

### **Estructura del Proyecto**
```
[Módulo]/
└── [Módulo].Proxies/
    ├── [Módulo].Proxies.csproj
    ├── Abstractions/
    │   └── I[Nombre]Proxy.cs
    ├── [Nombre]Proxy.cs
    ├── DependencyContainer.cs
    ├── bin/
    └── obj/
```

### **Configuración [Módulo].Proxies.csproj**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <!-- Paquetes base para DI y HTTP -->
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" />
    <PackageReference Include="Microsoft.Extensions.Http" />
    
    <!-- Agregar aquí paquetes NuGet específicos según el servicio a consumir -->
    <!-- Ejemplos: SDKs de terceros, clientes de BD, servicios cloud, etc. -->
  </ItemGroup>

  <ItemGroup>
    <!-- Referencia obligatoria a Entities -->
    <ProjectReference Include="..\..\Common\Entities\Entities.csproj" />
    
    <!-- Agregar aquí referencias a otros proyectos Common si es necesario -->
    <!-- Ejemplo: bibliotecas compartidas de lógica de negocio -->
  </ItemGroup>
</Project>
```

**Notas:**
- Agregar solo los paquetes NuGet necesarios para el servicio específico
- Las referencias a Common permiten reutilizar lógica compartida
- Todos los paquetes deben estar definidos en `Directory.Packages.props`

---

### **Reglas para Proxies:**
- **Interfaces deben ser `public`** - se inyectan desde otros proyectos
- **Implementaciones deben ser `internal`** - solo se accede a través de interfaces
- **SIEMPRE implementar su interfaz correspondiente** de `Abstractions/`
- Usar primary constructor con dependencias necesarias (ej: `HttpClient`, `ILogger<T>`, SDKs, etc.)
- Para HTTP: Definir constante `BaseRoute` con la ruta base de la API
- **TODOS los tipos (DTOs y ValueObjects) deben estar definidos en el proyecto Entities**
  - Los tipos de retorno deben basarse en la definición del endpoint/servicio
  - Ejemplo: Si el endpoint retorna `ApiResponse<ProductDto>`, usar `Task<ApiResponse<ProductDto>>`
- **Manejo de excepciones:**
  - Usar try-catch **SOLO para logging** del error
  - **SIEMPRE hacer `throw;`** después del log - las excepciones deben propagarse
  - No capturar la excepción sin relanzarla
- Para HTTP: Usar `ReadFromJsonAsync<T>()` para deserializar respuestas
- Para HTTP: Usar `PostAsJsonAsync()`, `PutAsJsonAsync()`, `DeleteAsync()` para modificar datos
- **Namespace:** `[Módulo].Proxies`
- **Namespace Interfaces:** `[Módulo].Proxies.Abstractions`
- **Un solo `return` por método** al final

---

## Abstractions - Interfaces de Proxies

**Cada Proxy debe tener su interfaz correspondiente en la carpeta `Abstractions/`:**

### **Reglas para Interfaces:**
- Definir interfaz en carpeta `Abstractions/`
- Nombre de interfaz: `I[Nombre]Proxy.cs`
- Incluir todos los métodos públicos del Proxy
- **Namespace:** `[Módulo].Proxies.Abstractions`
- Las interfaces permiten inyección de dependencias y testing

### **Ejemplo de Interfaz:**

**IProductProxy.cs:**
```csharp
using Entities.ValueObjects;
using Entities.Dtos;

namespace ProductModule.Proxies.Abstractions;

public interface IProductProxy
{   
    Task<ApiResponse<ProductDto>> GetProductByIdAsync(int productId);
    Task<ApiResponse<bool>> CreateProductAsync(ProductDto product);
}
```

**Nota:** Los tipos (`ApiResponse`, `ProductDto`) deben estar definidos en el proyecto Entities y coincidir con lo que retorna el endpoint/servicio.

---

## 🚀 Implementación de Proxies

### **Primary Constructor (OBLIGATORIO)**

Los Proxies **SIEMPRE** deben usar primary constructors e implementar su interfaz correspondiente:

**Ejemplo GET:**
```csharp
using Entities.ValueObjects;
using Entities.Dtos;
using Microsoft.Extensions.Logging;
using ProductModule.Proxies.Abstractions;
using using System.Net.Http.Json;

namespace ProductModule.Proxies;

internal class ProductProxy(HttpClient httpClient, ILogger<ProductProxy> logger) : IProductProxy
{
    private const string BaseRoute = "api/products";

    public async Task<ApiResponse<ProductDto>> GetProductByIdAsync(int productId)
    {
        ApiResponse<ProductDto> result;
        try
        {
            var response = await httpClient.GetAsync($"{BaseRoute}/{productId}");
            result = await response.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting product with ID {ProductId}", productId);
            throw; // SIEMPRE relanzar la excepción después del log
        }
        return result;
    }

    public async Task<ApiResponse<bool>> CreateProductAsync(ProductDto product)
    {
        ApiResponse<bool> result;
        try
        {
            var response = await httpClient.PostAsJsonAsync(BaseRoute, product);
            result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating product {ProductName}", product.Name);
            throw; // SIEMPRE relanzar la excepción después del log
        }
        return result;
    }
}
```

**Notas:**
- El tipo en `ReadFromJsonAsync<T>()` debe coincidir exactamente con el tipo de retorno (sin el `Task<>`)
- Los tipos deben estar definidos en Entities y basarse en la definición del endpoint
- El try-catch es solo para logging, siempre se debe hacer `throw;` para propagar la excepción
- Con el patrón de POST se pueden inferir PUT (`PutAsJsonAsync`) y DELETE (`DeleteAsync`)

---

## 📦 DependencyContainer - Registro de Servicios

**Cada proyecto Proxy debe tener un archivo `DependencyContainer.cs` para registrar sus servicios:**

### **Reglas para DependencyContainer:**
- Clase `public static` en la raíz del proyecto
- Método de extensión sobre `IServiceCollection`
- Nombre del método: `Add[Módulo]Proxies`
- **Recibir `Action<HttpClient>` como parámetro** para configurar la URL base del HttpClient
- Usar `AddHttpClient<T>()` para registrar cada Proxy que use HTTP
- Retornar `IServiceCollection` para permitir chaining
- **Namespace:** `[Módulo].Proxies`


### **Uso desde Program.cs:**
```csharp
builder.Services.AddProductModuleProxies(client =>
{
    client.BaseAddress = new Uri("https://api.example.com/");
});
```

**Notas:**
- El `Action<HttpClient>` permite configurar la URL base desde el proyecto principal
- Registrar todos los Proxies del módulo que usen HttpClient
- Si hay Proxies que usen otros tipos de servicios (SDKs, etc.), registrarlos con `AddScoped<Interfaz, Implementacion>()`
- La interfaz se registra automáticamente con `AddHttpClient<T>()` cuando se usa primary constructor



