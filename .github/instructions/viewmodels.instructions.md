---
description: Instrucciones para la creación y uso de proyectos ViewModels en la arquitectura del sistema.
applyTo: "**/*.ViewModels/**, **/*.ViewModels.csproj"
---

## 📦 Proyecto ViewModels

Los proyectos ViewModels son responsables de la **lógica de presentación, estado y validación**.

### **Estructura del Proyecto**
```
[Módulo]/
└── [Módulo].ViewModels/
    ├── [Módulo].ViewModels.csproj
    ├── DependencyContainer.cs
    ├── GlobalUsings.cs (opcional)
    ├── Abstractions/
    │   └── I[Nombre]ViewModel.cs
    ├── ViewModels/
    │   ├── [Nombre]ViewModel.cs
    │   └── ...
    ├── Models/
    │   ├── [Nombre]Model.cs
    │   └── ...
    ├── Adapters/
    │   ├── [Nombre]DtoToModelAdapter.cs
    │   └── ...
    ├── bin/
    └── obj/
```

### **Configuración [Módulo].ViewModels.csproj**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\[Módulo].Proxies\[Módulo].Proxies.csproj" />
  </ItemGroup>

</Project>
```


## 📊 Models - Modelos Locales de UI

**Models:** Modelos locales de UI con validaciones de DataAnnotations (no compartidos con el backend).

```csharp
using System.ComponentModel.DataAnnotations;

namespace [Módulo].ViewModels.Models;

public class ProductModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "El precio es requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Price { get; set; }
    
    [Required(ErrorMessage = "El stock es requerido")]
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }
}
```

### **Reglas para Models:**
- Usar DataAnnotations para validaciones (`[Required]`, `[Range]`, `[StringLength]`, etc.)
- Usar `ErrorMessage` con mensajes claros y descriptivos
- Propiedades con `get` y `set` (no usar primary constructors en Models)
- Un archivo por clase
- **Namespace:** `[Módulo].ViewModels.Models`

---

## 🔄 Adapters - Conversión de DTOs a Models

**Adapters:** Conversión de DTOs a Models usando extension methods (internal static class).

```csharp
using [Módulo].ViewModels.Models;
using Entities.Dtos;

namespace [Módulo].ViewModels.Adapters;

internal static class ProductDtoToModelAdapter
{
    public static ProductModel ToProductModel(this ProductDto dto)
    {
        return new ProductModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Price = dto.Price,
            Stock = dto.Stock
        };
    }

    public static List<ProductModel> ToProductModels(this IEnumerable<ProductDto> dtos)
    {
        return dtos.Select(dto => dto.ToProductModel()).ToList();
    }
    
    // Convertir Model a DTO (para crear/actualizar)
    public static CreateProductDto ToCreateProductDto(this ProductModel model)
    {
        return new CreateProductDto(
            name: model.Name,
            price: model.Price,
            stock: model.Stock
        );
    }
}
```

### **Reglas para Adapters:**
- **SIEMPRE** usar `internal static class`
- Métodos de extensión para convertir DTOs a Models
- Métodos de extensión para convertir Models a DTOs
- Incluir métodos para listas (`ToProductModels`)
- Un adapter por tipo de entidad
- Pasar parámetros adicionales cuando sea necesario (ej: `currentUserId` para marcar mensajes)
- Nombres descriptivos: `[Entity]DtoToModelAdapter` o `[Entity]Adapter`
- **Namespace:** `[Módulo].ViewModels.Adapters`

---

## 🔌 Abstractions - Interfaces de ViewModels

**Cada ViewModel debe tener su interfaz correspondiente en la carpeta `Abstractions/`:**

### **Reglas para Interfaces:**
- Definir interfaz en carpeta `Abstractions/`
- Nombre de interfaz: `I[Nombre]ViewModel.cs`
- Incluir todos los métodos públicos del ViewModel
- Incluir propiedades públicas (excepto eventos)
- **Namespace:** `[Módulo].ViewModels.Abstractions`
- Las interfaces permiten inyección de dependencias y testing

### **Ejemplo de Interfaz para ViewModel:**

**IProductViewModel.cs:**
```csharp
using [Módulo].ViewModels.Models;

namespace [Módulo].ViewModels.Abstractions;

public interface IProductViewModel
{
    // Eventos
    event EventHandler<string> OnFailure;
    
    // Propiedades
    int CurrentPage { get; }
    int ItemsPerPage { get; }
    int TotalPages { get; }
    string SearchTerm { get; set; }
    List<ProductModel> Products { get; }
    ProductModel Model { get; set; }
    bool IsLoading { get; }
    
    // Métodos
    Task InitializeViewModel();
    Task SearchAsync();
    Task NextPageAsync();
    Task PreviousPageAsync();
}
```

**Importante sobre Eventos:** Los eventos de notificación de errores (`event EventHandler<string> OnFailure`) **DEBEN** incluirse en la interfaz para que las Views puedan suscribirse a través de la interfaz inyectada.

---

## 🎯 ViewModels - Lógica de Presentación

### **Separación de Responsabilidades por Capa:**

#### **ViewModels (Lógica Compartible):**
- Lógica de negocio y estado
- Llamadas a Proxies/APIs
- Validaciones de datos (usando Models)
- Transformación de datos (usando Adapters)
- Gestión de estado de carga
- **Solo código que funcione en cualquier plataforma (Blazor, MAUI, Desktop)**
- **Los métodos deben recibir parámetros en lugar de asumir APIs de plataforma**
- Ejemplo: `SearchProductByBarcodeAsync(string barcode)` en lugar de `ScanBarcodeAsync()` que asume escaneo

### **Primary Constructor (OBLIGATORIO)**

Los ViewModels **SIEMPRE** deben usar primary constructors e implementar su interfaz correspondiente:
`

### **Reglas para ViewModels:**
- **Interfaces deben ser `public`** - se inyectan desde Views u otros proyectos
- **Implementaciones deben ser `internal`** - solo se accede a través de interfaces
- **SIEMPRE implementar su interfaz correspondiente** de `Abstractions/`
- **Inyectar interfaces de Proxies**, NO las implementaciones concretas
- Usar primary constructor con interfaces de Proxy(s) necesarios y `ILogger<T>`
- **SIEMPRE incluir `EventHandler<string> OnFailure`** para notificar errores a la UI
- **NO incluir eventos OnSuccess** - los métodos retornan valores booleanos o datos, la Vista decide si mostrar mensaje de éxito
- **SIEMPRE incluir método `InitializeViewModel()`** para inicialización asíncrona
- **NO incluir validaciones directamente** (usar clases Model con DataAnnotations)
- **NO manejar ErrorMessage como propiedad** - usar OnFailure event para notificar
- **NO manejar SuccessMessage** - la Vista verifica el valor de retorno y muestra su propio mensaje
- **NO inicializar propiedades que pueden ser null** - no usar `= string.Empty`, `= new()`, etc.
- **NO incluir lógica específica de plataforma** - solo lógica compartible entre Blazor, MAUI, etc.
- **Evitar APIs de navegador, teléfono, navegación UI** - esa lógica debe estar en Views
- ViewModels deben ser independientes de la plataforma y enfocarse en lógica de negocio y estado
- Instanciar Models con validaciones como propiedades públicas cuando sea necesario
- Incluir propiedad `IsLoading` para estado de carga
- **Validar resultados usando `HandlerRequestResult.Success`**
- **Usar operador null-coalescing para asignar valores vacíos**: `Products = result.SuccessValue?.Items.ToProductModels() ?? [];`
- Verificar `if (!result.Success)` o `if (result.Success)` para logging y OnFailure
- Registrar warnings con `LogWarning` cuando el proxy retorna `!result.Success`
- Invocar `OnFailure?.Invoke(this, result.ErrorMessage)` cuando hay error del proxy
- Usar Adapters para convertir DTOs a Models
- **NO usar try-catch en ViewModels** - las excepciones deben propagarse a la capa de Views
- Los Proxies ya hacen logging de errores antes de relanzar excepciones
- La UI es responsable de suscribirse a OnFailure y mostrar los errores al usuario
- La UI es responsable de verificar el valor de retorno de los métodos y mostrar mensajes de éxito desde sus propios recursos
- La UI es responsable de manejar excepciones con try-catch y mostrar mensajes genéricos de error
- **Namespace ViewModels:** `[Módulo].ViewModels.ViewModels`
- **Namespace Interfaces:** `[Módulo].ViewModels.Abstractions`

---

## 🎯 Separación de Responsabilidades CQRS en ViewModels

Los ViewModels deben separarse en **múltiples ViewModels especializados por responsabilidad funcional**:

### **Reglas de Separación:**
- **SearchViewModel / QueryViewModel**: Solo para **consultas** (queries) - Cargar y mostrar datos
- **ActionViewModel / CommandViewModel**: Solo para **acciones** (commands) - Crear, actualizar, eliminar
- **DetailViewModel**: Para ver detalles de una entidad específica
- **Las propiedades expuestas deben usar Models**, NO DTOs
- Cada ViewModel se enfoca en una responsabilidad específica
- Los ViewModels de consulta usan Adapters para convertir DTOs a Models

### **Estructura recomendada:**
```
[Módulo].ViewModels/
├── ViewModels/
│   ├── [Entity]SearchViewModel.cs    # Consultas y búsquedas
│   ├── [Entity]ActionViewModel.cs    # Acciones (crear, actualizar, eliminar)
│   └── [Entity]DetailViewModel.cs    # Detalles de una entidad
├── Models/
│   └── [Entity]Model.cs
└── Adapters/
    └── [Entity]Adapter.cs
```

### **Ventajas de separar ViewModels:**
- **Responsabilidad única**: Cada ViewModel tiene una función clara
- **Reutilizable**: Los ViewModels pueden usarse en diferentes vistas
- **Testeable**: Más fácil hacer pruebas unitarias
- **Mantenible**: Cambios en consultas no afectan acciones
- **Escalable**: Fácil agregar nuevas funcionalidades

---

