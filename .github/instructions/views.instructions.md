---
description: Instrucciones para la creación y uso de proyectos Views en la arquitectura del sistema.
applyTo: "**/*.Views/**, **/*.Views.csproj, **/Pages/**/*.razor, **/Pages/**/*.razor.cs"
---

## 📦 Proyecto Views

Los proyectos Views son responsables de los **componentes de UI (Razor para Blazor o XAML para MAUI)**.

### **Estructura del Proyecto (Blazor)**
```
[Módulo]/
└── [Módulo].Views/
    ├── [Módulo].Views.csproj
    ├── Pages/
    │   ├── [Nombre]Page.razor
    │   └── [Nombre]Page.razor.cs
    ├── bin/
    └── obj/
```

### **Configuración [Módulo].Views.csproj (Blazor)**
```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  
  <ItemGroup>
    <SupportedPlatform Include="browser" />
    <SupportedPlatform Include="ios" />
    <SupportedPlatform Include="android" />
    <SupportedPlatform Include="windows" />
  </ItemGroup>

    <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Components.Web" />
  </ItemGroup>
  
  <ItemGroup>
    <ProjectReference Include="..\[Módulo].ViewModels\[Módulo].ViewModels.csproj" />
  </ItemGroup>
</Project>
```

---

## 🎨 Separación de Responsabilidades

### **Views (Lógica Específica de Plataforma):**
- Componentes UI (Razor/XAML)
- Navegación entre páginas
- APIs del navegador (localStorage, sessionStorage, geolocation)
- APIs de dispositivo (cámara, GPS, sensores, escáner de códigos de barras)
- Interacciones específicas de UI
- Manejo de eventos de plataforma
- **Obtención de datos específicos de plataforma que se pasan al ViewModel**
- Ejemplo: En web usar input manual, en MAUI usar cámara para escanear, ambos pasan el resultado al ViewModel
- **Código que depende de la plataforma de ejecución**

---

## 🎨 Estilos en Views

### **Reglas:**
- **Usar EXCLUSIVAMENTE clases de Bootstrap** para todos los componentes y estilos
- **NO crear CSS custom** - no usar archivos `.razor.css` 
- **NO agregar estilos inline** - solo clases de Bootstrap
- Usar Bootstrap Icons para iconografía: `<i class="bi bi-save"></i>`

---

## 📄 Code-Behind en Blazor (OBLIGATORIO)

**SIEMPRE separar el código C# en archivos .razor.cs**.

### **❌ NO HACER (código en .razor):**
```razor
@page "/products"
@inject ProductViewModel ViewModel
@inject NavigationManager Navigation

<div class="container">
    <!-- markup -->
</div>

@code {
    private async Task LoadProducts()
    {
        await ViewModel.LoadAsync();
    }
}
```

### **✅ HACER (código en .razor.cs):**

**ProductListPage.razor:**
```razor
@page "/products"

<div class="container">
    <!-- markup -->
</div>
```

**ProductListPage.razor.cs:**
```csharp
using Microsoft.AspNetCore.Components;
using ProductModule.ViewModels.Abstractions;

namespace ProductModule.Views.Pages;

public partial class ProductListPage(IProductViewModel viewModel, NavigationManager navigation)
{
    private string SuccessMessage { get; set; }
    private string ErrorMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        viewModel.OnFailure += HandleFailure;
        
        try
        {
            await viewModel.InitializeViewModel();
        }
        catch (Exception)
        {
            ErrorMessage = "Error al cargar los productos";
        }
    }

    private async Task LoadProducts()
    {
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            
            var success = await viewModel.LoadAsync();
            
            if (success)
            {
                SuccessMessage = "Productos cargados exitosamente";
            }
        }
        catch (Exception)
        {
            ErrorMessage = "Error al cargar productos";
        }
        
        StateHasChanged();
    }

    private void HandleFailure(object sender, string errorMessage)
    {
        ErrorMessage = errorMessage;
        StateHasChanged();
    }

    public void Dispose()
    {
        viewModel.OnFailure -= HandleFailure;
    }
}
```

### **Reglas para Code-Behind:**
- Usar `public partial class` con el mismo nombre del archivo .razor
- Namespace debe coincidir con la estructura de carpetas
- **Inyectar interfaces de ViewModels**, NO las implementaciones concretas
- **SIEMPRE usar primary constructor para inyecciones de dependencias**
- Parámetros del constructor en camelCase
- **Usar directamente los parámetros del constructor** - no es necesario exponerlos como propiedades públicas
- Parámetros de componente con `[Parameter]` attribute
- Métodos de ciclo de vida: `OnInitializedAsync`, `OnParametersSetAsync`, etc.
- **Variables y propiedades privadas en PascalCase** (usar propiedades en lugar de campos privados)
- **No inicializar propiedades que pueden ser null** - no usar nullable reference types
- Ejemplo: `private string ErrorMessage { get; set; }` en lugar de `private string ErrorMessage { get; set; } = string.Empty;`
- **SIEMPRE suscribirse a `ViewModel.OnFailure`** en `OnInitializedAsync`
- **SIEMPRE des-suscribirse en `Dispose()`**
- **Manejar excepciones con try-catch** - la Vista es responsable de atrapar errores
- **Verificar valor de retorno de métodos del ViewModel** para mostrar mensajes de éxito
- **Usar `StateHasChanged()`** para actualizar la UI cuando cambie el estado
- **Namespace:** `[Módulo].Views.Pages`

---


## 🔧 Convenciones de Código

### **Naming:**
- **Páginas Razor:** `[Nombre]Page.razor` + `[Nombre]Page.razor.cs`
- **Namespace:** `[Módulo].Views.Pages`

### **Inicialización de Propiedades:**
- **No inicializar propiedades que pueden ser null** - no agregar `= string.Empty`, `= new()`, `= null`, etc.
- **No usar nullable reference types (`?`)** - el `?` debe ser agregado explícitamente por el usuario solo cuando sea necesario
- **Excepción:** Solo inicializar cuando el valor por defecto NO sea null

### **Asignación de Valores dentro de Métodos:**
- **NUNCA asignar `null` a variables/propiedades dentro de métodos** - usar valores por defecto apropiados
- Para strings: usar `string.Empty` en lugar de `null`
- Para números: usar `0` en lugar de `null`
- Para colecciones: usar `[]` o `new List<T>()` en lugar de `null`

---
