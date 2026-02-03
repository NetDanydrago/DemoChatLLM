# Arquitectura Client - Convenciones y Reglas Generales

Este documento contiene las convenciones de código, reglas de arquitectura y estándares generales del proyecto.

---

## ⚡ Requisitos de Plataforma

**IMPORTANTE:** Todos los proyectos deben usar **.NET 10.0** como target framework para aprovechar las últimas características:

- **Primary Constructors**: Simplifica la inyección de dependencias
- **Collection Expressions**: Sintaxis simplificada para colecciones `[]`
- **Interceptors**: Mejoras en generación de código para Blazor
- **Performance Improvements**: Blazor WebAssembly con mejor rendimiento y menor tamaño
- **Nullable Reference Types**: Análisis mejorado de nullabilidad

**Target Framework:** `net10.0` en todos los .csproj

---

## 📐 Arquitectura del Proyecto

El proyecto sigue una **arquitectura modular vertical slice** con separación de responsabilidades en tres capas por módulo:

```
NombreProyecto.sln
├── [Módulo]/
│   ├── [Módulo].Proxies/          # Capa de comunicación con APIs
│   ├── [Módulo].ViewModels/       # Lógica de presentación y estado
│   └── [Módulo].Views/            # Componentes de UI
├── Common/                        # Código compartido
│   └── Entities/                  # DTOs y ValueObjects
└── NombreProyecto.Web/            # Proyecto principal Blazor WebAssembly
```

---

##  Convenciones de Código

### **Naming General:**
- **Clases:** `PascalCase`
- **Métodos:** `PascalCase`
- **Propiedades:** `PascalCase`
- **Variables privadas:** `PascalCase`
- **Campos readonly:** `PascalCase`
- **Parámetros:** `camelCase`

### **Inicialización de Propiedades:**
- **No inicializar propiedades que pueden ser null** - no agregar `= string.Empty`, `= new()`, `= null`, etc.
- **No usar nullable reference types (`?`)** - el `?` debe ser agregado explícitamente por el usuario solo cuando sea necesario
- **Excepción:** Solo inicializar cuando el valor por defecto NO sea null

### **Asignación de Valores dentro de Métodos:**
- **NUNCA asignar `null` a variables/propiedades dentro de métodos** - usar valores por defecto apropiados
- Para strings: usar `string.Empty` en lugar de `null`
- Para números: usar `0` en lugar de `null`
- Para colecciones: usar `[]` o `new List<T>()` en lugar de `null`

### **Estructura de Métodos:**
- **Un solo `return` por método** - debe estar al final del método
- Usar variables locales para almacenar resultados intermedios
- Evitar múltiples puntos de salida (`return` en medio del método)

### **Primary Constructors (C# 12+):**
- **SIEMPRE usar primary constructors** para ViewModels y Proxies
- Eliminar campos privados cuando se usen primary constructors
- Usar directamente el parámetro del constructor en lugar del campo

---

## � CSS y Estilos

### **CSS Aislado del Proyecto Web**

**IMPORTANTE:** El proyecto Blazor WebAssembly es el **ÚNICO** que debe mantener CSS aislado (`wwwroot/css/app.css`).

**Propósito:** Este archivo contiene únicamente los estilos para el `blazor-error-ui` que Blazor crea por defecto.

**Reglas:**
- **Solo usar clases del framework CSS** (Bootstrap, MudBlazor, etc.) para todos los componentes
- Este CSS solo existe para el error UI de Blazor
- En `index.html`, el `app.css` debe cargarse **antes** del framework CSS

---

## 🎯 Arquitectura Mobile-First

1. **Usar EXCLUSIVAMENTE las clases del framework CSS** diseñadas para mobile-first
2. **NO crear estilos CSS custom** - confiar en el framework CSS para responsividad
3. Diseñar primero para pantallas móviles (320px - 480px)
4. Aprovechar las utilidades responsive del framework
5. Componentes UI táctiles (botones > 44px)
6. Evitar hover-only interactions
7. Usar componentes nativos del framework que ya son responsive

---

## 📦 Gestión Central de Paquetes NuGet

El proyecto usa **Central Package Management** para gestionar versiones de paquetes NuGet de forma centralizada.

**Archivo `Directory.Packages.props` (raíz de la solución):**
```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <ItemGroup>
    <!-- ASP.NET Core / Blazor -->
    <PackageVersion Include="Microsoft.AspNetCore.Components.Web" Version="10.0.2" />
    <PackageVersion Include="Microsoft.AspNetCore.Components.WebAssembly" Version="10.0.2" />
    <PackageVersion Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="10.0.2" />
    
    <!-- Extensions -->
    <PackageVersion Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="10.0.2" />
    <PackageVersion Include="Microsoft.Extensions.Http" Version="10.0.2" />
    <PackageVersion Include="System.Net.Http.Json" Version="10.0.2" />
  </ItemGroup>
</Project>
```

**En los archivos .csproj (SIN versiones):**
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Http" />
  <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" />
</ItemGroup>
```

**Reglas:**
- **NUNCA** agregar versiones en los .csproj individuales
- **SIEMPRE** actualizar versiones en `Directory.Packages.props`
- Usar `<PackageVersion>` en lugar de `<PackageReference>` en el archivo central
- **TODOS los paquetes usados en los .csproj deben estar definidos en `Directory.Packages.props`**

---
