# Feature/Módulo - Instrucciones de Creación

## 📂 Estructura de un Módulo

Cada módulo funcional debe tener exactamente **3 proyectos** siguiendo la arquitectura modular vertical slice:

```
[Módulo]/
├── [Módulo].Proxies/          # Capa de comunicación con APIs
├── [Módulo].ViewModels/       # Lógica de presentación y estado (independiente de plataforma)
└── [Módulo].Views/            # Componentes de UI (Razor/XAML) y lógica específica de plataforma
```

### **Separación de Responsabilidades por Capa:**

#### **ViewModels (Lógica Compartible):**
- Lógica de negocio y estado
- Llamadas a Proxies/APIs
- Validaciones de datos (usando Models)
- Transformación de datos (usando Adapters)
- Gestión de estado de carga
- **Solo código que funcione en cualquier plataforma (Blazor, MAUI, Desktop)**
- **Los métodos deben recibir parámetros en lugar de asumir APIs de plataforma**

#### **Views (Lógica Específica de Plataforma):**
- Componentes UI (Razor/XAML)
- Navegación entre páginas
- APIs del navegador (localStorage, sessionStorage, geolocation)
- APIs de dispositivo (cámara, GPS, sensores, escáner de códigos de barras)
- Interacciones específicas de UI
- Manejo de eventos de plataforma
- **Obtención de datos específicos de plataforma que se pasan al ViewModel**
- **Código que depende de la plataforma de ejecución**

---

## 🏗️ Patrón DependencyContainer

**Cada proyecto Proxies y ViewModels debe tener un `DependencyContainer.cs`** con un método de extensión para registrar servicios.

---

## 🚀 Integración con el Proyecto Principal

### **1. Configuración Program.cs**

Registrar el módulo en el contenedor de DI:

```csharp
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NombreProyecto.Web;

// Imports del módulo
using [Módulo].Proxies;
using [Módulo].ViewModels;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient base
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
});

// Registro del módulo (patrón DependencyContainer)
builder.Services.Add[Módulo]ViewModels();
builder.Services.Add[Módulo]Proxies(
    proxy =>
    {
        proxy.BaseAddress = new Uri(builder.Configuration["WebApiAddress"]);
    },
    provider => provider.GetRequiredService<AuthenticationHandler>() // Handler para autenticación
);

await builder.Build().RunAsync();
```

### **2. Configuración App.razor**

**IMPORTANTE:** El Router debe incluir `AdditionalAssemblies` para descubrir las rutas del módulo:

```razor
@using Microsoft.AspNetCore.Components.Routing
@using [NombreProyecto].Web.Shared
<Router AppAssembly="@typeof(App).Assembly" 
        AdditionalAssemblies="new[] { typeof([Módulo].Views.Pages.[Nombre]Page).Assembly }">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
        <FocusOnNavigate RouteData="@routeData" Selector="h1" />
    </Found>
    <NotFound>
        <PageTitle>Not found</PageTitle>
        <LayoutView Layout="@typeof(MainLayout)">
            <p role="alert">Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</Router>
```

**Reglas:**
- **SIEMPRE incluir el using `@using Microsoft.AspNetCore.Components.Routing`** al inicio del archivo
- **SIEMPRE incluir el using `@using [NombreProyecto].Web.Shared`** para acceder al MainLayout (ajustar según el nombre del proyecto)
- **SIEMPRE incluir `AdditionalAssemblies`** cuando se usan módulos .Views
- Referenciar cualquier página del módulo .Views para obtener su Assembly
- Si hay múltiples módulos, incluir todos los assemblies en el array
- Sin esto, las rutas `@page` de los módulos no serán descubiertas

### **3. Referencia en .csproj Principal**

Agregar referencia solo al proyecto `.Views` del módulo:

```xml
<ItemGroup>
  <!-- Referenciar solo el proyecto .Views del módulo -->
  <ProjectReference Include="..\[Módulo]\[Módulo].Views\[Módulo].Views.csproj" />
</ItemGroup>
```

---

## 🎨 Framework CSS - Bootstrap

**IMPORTANTE:** Todos los proyectos SIEMPRE usan **Bootstrap** como framework CSS.

### **Reglas:**
- **Usar EXCLUSIVAMENTE clases de Bootstrap** para estilos y componentes
- **NO crear CSS custom** - confiar en Bootstrap para responsividad
- Usar Bootstrap CDN en lugar de paquetes NuGet
- Incluir Bootstrap Icons para iconografía
- Arquitectura Mobile-First (Bootstrap es mobile-first por defecto)

### **Configuración en index.html:**

```html
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>NombreProyecto</title>
    <base href="/" />
    <link href="css/app.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.0/font/bootstrap-icons.css" rel="stylesheet">
    <link href="NombreProyecto.Web.styles.css" rel="stylesheet" />
</head>
```

### **Componentes Esenciales:**

**MainLayout.razor con Bootstrap:**
```razor
@inherits LayoutComponentBase

<div class="d-flex flex-column min-vh-100">
    <NavMenu />
    
    <main class="flex-fill">
        <div class="container-fluid py-4">
            @Body
        </div>
    </main>
    
    <footer class="bg-light border-top py-3 mt-auto">
        <div class="container-fluid text-center text-muted">
            <small>&copy; @DateTime.Now.Year NombreProyecto</small>
        </div>
    </footer>
</div>
```

**NavMenu.razor con Bootstrap Navbar:**
```razor
<nav class="navbar navbar-expand-lg navbar-dark bg-primary">
    <div class="container-fluid">
        <a class="navbar-brand" href="">
            <i class="bi bi-house-door-fill me-2"></i>
            NombreProyecto
        </a>
        <button class="navbar-toggler" type="button" @onclick="ToggleNavMenu">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="@NavMenuCssClass navbar-collapse">
            <ul class="navbar-nav me-auto">
                <li class="nav-item">
                    <NavLink class="nav-link" href="" Match="NavLinkMatch.All">
                        <i class="bi bi-house-door-fill me-1"></i> Home
                    </NavLink>
                </li>
            </ul>
        </div>
    </div>
</nav>

@code {
    private bool collapseNavMenu = true;
    private string NavMenuCssClass => collapseNavMenu ? "collapse" : string.Empty;
    
    private void ToggleNavMenu()
    {
        collapseNavMenu = !collapseNavMenu;
    }
}
```

### **Clases Bootstrap Más Usadas:**

**Layout y Grid:**
- `.container-fluid` - Contenedor responsive full-width
- `.row` - Fila del grid
- `.col-*` - Columnas responsive (col-12, col-md-6, col-lg-4)

**Flexbox:**
- `.d-flex` - Display flex
- `.flex-column` - Dirección vertical
- `.justify-content-*` - Alineación horizontal
- `.align-items-*` - Alineación vertical

**Espaciado:**
- `.p-*`, `.py-*`, `.px-*` - Padding
- `.m-*`, `.my-*`, `.mx-*` - Margin
- `.gap-*` - Espaciado en flex/grid

**Componentes:**
- `.btn`, `.btn-primary`, `.btn-secondary` - Botones
- `.card`, `.card-body` - Tarjetas
- `.table`, `.table-striped` - Tablas
- `.form-control`, `.form-label` - Formularios
- `.alert`, `.alert-*` - Alertas

---

## ✅ Checklist de Creación de Módulo

Para cada nuevo módulo funcional:

- [ ] Crear carpeta `[Módulo]/`
- [ ] Crear `[Módulo].Proxies/`
  - [ ] Crear .csproj con SDK `Microsoft.NET.Sdk`
  - [ ] Agregar paquetes NuGet necesarios
  - [ ] Crear `[Nombre]Proxy.cs`
  - [ ] Crear `DependencyContainer.cs` con método `Add[Módulo]Proxies`
  - [ ] Agregar referencia a `Common/Entities`
- [ ] Crear `[Módulo].ViewModels/`
  - [ ] Crear .csproj con SDK `Microsoft.NET.Sdk`
  - [ ] Crear carpeta `ViewModels/`
  - [ ] Crear `DependencyContainer.cs` con método `Add[Módulo]ViewModels`
  - [ ] Agregar referencia a `[Módulo].Proxies`
- [ ] Crear `[Módulo].Views/`
  - [ ] Crear .csproj con SDK `Microsoft.NET.Sdk.Razor` (Blazor)
  - [ ] Crear carpeta `Pages/`
  - [ ] Crear páginas `.razor` y `.razor.cs`
  - [ ] Agregar referencias a `Common.Views` y `[Módulo].ViewModels`
- [ ] Registrar en `Program.cs`:
  - [ ] Agregar usings del módulo
  - [ ] Llamar a `Add[Módulo]ViewModels()`
  - [ ] Llamar a `Add[Módulo]Proxies()`
- [ ] Agregar referencia al proyecto `.Views` en `NombreProyecto.Web.csproj`
- [ ] Agregar assembly del módulo en `App.razor` → `AdditionalAssemblies`

---

## 📋 Convenciones de Nomenclatura

### **Nombres de Módulos:**
- **PascalCase** sin separadores
- Nombres descriptivos del dominio de negocio
- Ejemplos: `UserManager`, `OrderManagement`, `InventoryControl`

### **Proyectos del Módulo:**
- `[Módulo].Proxies` - Comunicación con API
- `[Módulo].ViewModels` - Lógica de presentación
- `[Módulo].Views` - Componentes UI

---
