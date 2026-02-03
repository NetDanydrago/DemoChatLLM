---
description: Instrucciones para la creación y uso del proyecto Entities que contiene DTOs y ValueObjects comunes.
applyTo: '**/Entities/**, **/Entities.csproj, **/Dtos/**, **/ValueObjects/**'
---


## 📂 Proyecto Entities (Common)

El proyecto Entities contiene los elementos compartidos por toda la aplicación:
- **DTOs** (Data Transfer Objects) - Se crean según lo que pide y devuelve cada endpoint del API
- **ValueObjects** (Objetos de valor reutilizables) - Para patrones comunes de respuesta

### **Estructura del Proyecto**
```
Common/
└── Entities/
    ├── Entities.csproj
    ├── Dtos/
    │   └── [Los DTOs necesarios según los endpoints]
    └── ValueObjects/
        └── [ValueObjects comunes para respuestas]
```

### **Configuración Entities.csproj**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

---

## 📦 DTOs - Definición Basada en Endpoints

**Los DTOs se crean según lo que cada endpoint del API requiere y devuelve:**

- **Request DTOs:** Modelan los parámetros que el endpoint espera recibir
- **Response DTOs:** Modelan los datos que el endpoint devuelve

### **Convención de Primary Constructor**

Los DTOs **SIEMPRE** deben usar primary constructors con propiedades de solo lectura:

```csharp
namespace Entities.Dtos;

// Ejemplo: DTO para petición de login
public class LoginRequest(string username, string password)
{
    public string Username => username;
    public string Password => password;
}

// Ejemplo: DTO para respuesta del API
public class UserDto(int id, string name, string email)
{
    public int Id => id;
    public string Name => name;
    public string Email => email;
}
```

**Reglas para DTOs:**
- Usar primary constructor con todos los parámetros en camelCase
- Propiedades de solo lectura (solo `get` con expresión `=>`)
- Nombres de propiedades en PascalCase
- Un archivo por clase
- **Namespace:** `Entities.Dtos`
- **Crear solo los DTOs necesarios según los endpoints del API**

---

## 🎯 ValueObjects - Patrones de Respuesta Comunes

Los ValueObjects encapsulan patrones de respuesta reutilizables en toda la aplicación.

**Solo crear ValueObjects cuando se identifique un patrón repetitivo en las respuestas del API.**


## 🔧 Convenciones de Código

### **Naming:**
- **DTOs:** Nombres descriptivos según el endpoint (ej: `LoginRequest`, `UserDto`, `CreateOrderDto`)
- **ValueObjects:** Nombres genéricos del patrón (ej: `PaginatedResponse`, `HandlerRequestResult`)
- **Namespace:** `Entities.Dtos` o `Entities.ValueObjects`

### **Inicialización de Propiedades:**
- **En DTOs:** Usar propiedades de solo lectura con primary constructor (solo `=>`)
- **En ValueObjects:** Inicializar con valores por defecto cuando sea necesario
- **Excepción:** Solo inicializar cuando el valor por defecto NO sea null

### **Principio Guía:**
- **DTOs:** Uno por cada estructura de datos única del API (request/response)
- **ValueObjects:** Uno por cada patrón repetitivo identificado
- **No crear elementos "por si acaso"** - solo lo que se necesita según el API

---
