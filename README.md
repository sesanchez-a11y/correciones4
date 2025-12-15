# EduMentor - Plataforma de Tutorías Virtuales

##  Descripción General

EduMentor es una plataforma web de comercio electrónico especializada en tutorías y cursos en línea. El proyecto implementa una arquitectura de tres capas con autenticación JWT segura, gestión de usuarios con BCrypt, y un frontend responsivo.

##  Arquitectura

### Estructura de Capas

```
┌─────────────────────────────────────────┐
│         Frontend (HTML/CSS/JS)           │
│    - Autenticación con JWT               │
│    - Perfil de usuario                   │
│    - Gestión de cursos y reservas        │
└────────────────────┬────────────────────┘
                     │ HTTP/REST
┌────────────────────▼────────────────────┐
│         API REST (ASP.NET Core)          │
│  Controllers (ControladorDeSesion, etc)  │
│  - Endpoints de auth (register, login)   │
│  - Endpoint protegido /me                │
└────────────────────┬────────────────────┘
                     │
┌────────────────────▼────────────────────┐
│     Repository Pattern (IUsuarioRepository)  │
│  - InMemoryUsuarioRepository (dev)       │
│  - MongoUsuarioRepository (producción)   │
└────────────────────┬────────────────────┘
                     │
┌────────────────────▼────────────────────┐
│     Modelos de Dominio (Usuarios)        │
│  - Usuario (base)                        │
│  - Estudiante (hereda de Usuario)        │
│  - Tutor (hereda de Usuario)             │
└─────────────────────────────────────────┘
```

### Patrones Implementados

1. **Repository Pattern**: Abstracción de la capa de datos a través de `IUsuarioRepository`
2. **Dependency Injection**: Configuración en `Program.cs` usando el contenedor DI de ASP.NET Core
3. **Observer Pattern**: Usado en la clase `Reserva` para notificaciones
4. **Strategy Pattern**: `IPrecioStrategy` para cálculo de precios

##  Autenticación y Seguridad

### Flujo de Autenticación

```
1. Registro (POST /api/ControladorDeSesion/register)
   └─ Cliente envía: Email, Nombre, Contraseña, Rol
   └─ Backend: Valida duplicados, hashea contraseña con BCrypt
   └─ Retorna: Datos públicos del usuario

2. Login (POST /api/ControladorDeSesion/login)
   └─ Cliente envía: Email, Contraseña
   └─ Backend: Valida credenciales, genera JWT
   └─ Retorna: Token JWT + Datos públicos

3. Acceso Protegido (GET /api/ControladorDeSesion/me)
   └─ Cliente envía: Header `Authorization: Bearer <token>`
   └─ Backend: Valida firma JWT, extrae email del token
   └─ Retorna: Datos del usuario autenticado
```

### Configuración JWT

```json
"Jwt": {
  "Key": "cambiar_esta_clave_por_una_mas_segura_en_produccion_!",
  "Issuer": "EduMentor",
  "Audience": "EduMentorUsers",
  "ExpiryMinutes": 120
}
```

### Seguridad de Contraseñas

- **Algoritmo**: BCrypt con trabajo factor = 12 (por defecto)
- **Librería**: `BCrypt.Net-Next` NuGet
- **Hash Storage**: Se almacena solo el hash, nunca la contraseña original
- **Validación**: Se usa `BCrypt.Verify()` para comparar al autenticar

##  Dependencias del Proyecto

### Backend (TutoriasDeClases)
- **Framwork**: ASP.NET Core 9.0
- **Autenticación**: `Microsoft.AspNetCore.Authentication.JwtBearer` 8.0
- **Seguridad**: `BCrypt.Net-Next` 4.0.3
- **Base de Datos**: `MongoDB.Driver` 2.28.0 (opcional)
- **Logging**: Integrado en ASP.NET Core

### Tests (TutoriasDeClases.Tests)
- **Framework**: xUnit 2.7.0
- **Mocking**: Moq 4.20.70
- **SDK**: Microsoft.NET.Test.Sdk 17.10.0
- **Configuración**: Microsoft.Extensions.Configuration 8.0.0

##  Cómo Ejecutar

### Requisitos
- .NET 9 SDK instalado
- Visual Studio Code o Visual Studio (opcional)
- MongoDB (opcional, para producción)

### Pasos

1. **Clonar o descargar el proyecto**
   ```bash
   cd "c:\tareas\PROYECTO SOFTWARE2"
   ```

2. **Restaurar dependencias**
   ```bash
   cd TutoriasDeClases
   dotnet restore
   ```

3. **Ejecutar el servidor**
   ```bash
   dotnet run
   ```
   - Backend estará disponible en: `http://localhost:5000`
   - Frontend se sirve desde: `http://localhost:5000/` (archivos estáticos de `carpeta/`)

4. **Ejecutar los tests**
   ```bash
   cd ..\TutoriasDeClases.Tests
   dotnet test
   ```

##  Estructura del Proyecto

```
c:\tareas\PROYECTO SOFTWARE2\
├── TutoriasDeClases/              # Backend (ASP.NET Core)
│   ├── Program.cs                 # Configuración de servicios
│   ├── Controllers/
│   │   ├── ControladorDeSesion.cs # Endpoints de autenticación
│   │   └── ReservasController.cs  # Gestión de reservas
│   ├── Modelos/
│   │   ├── Usuario.cs
│   │   ├── Estudiante.cs
│   │   ├── Tutor.cs
│   │   └── ...
│   ├── Interfaces/
│   │   ├── IUsuarioRepository.cs
│   │   └── ...
│   ├── Repositories/
│   │   ├── InMemoryUsuarioRepository.cs
│   │   └── MongoUsuarioRepository.cs
│   └── appsettings.json           # Configuración (JWT, DB, etc)
│
├── TutoriasDeClases.Tests/        # Tests (xUnit)
│   ├── InMemoryUsuarioRepositoryTests.cs
│   ├── ControladorDeSesionTests.cs
│   └── TutoriasDeClases.Tests.csproj
│
├── carpeta/                       # Frontend (HTML/CSS/JS)
│   ├── archivoshtml/
│   │   ├── inicio.html
│   │   ├── perfil.html
│   │   ├── cursos.html
│   │   └── ...
│   ├── archivosjs/
│   │   ├── inicio.js
│   │   ├── perfil.js
│   │   └── ...
│   └── archivoscss/
│       ├── inicio.css
│       ├── perfil.css
│       └── ...
│
└── README.md                      # Este archivo
```

## Pruebas Unitarias

### Cobertura Actual

**Total de Tests**: 11 (100% pasadas)

#### InMemoryUsuarioRepository (7 tests)
- `AddAsync_ShouldAddUserSuccessfully` - Agregar usuario correctamente
- `FindByEmailAsync_ShouldReturnUserIfExists` - Buscar usuario existente
- `FindByEmailAsync_ShouldReturnNullIfUserDoesNotExist` - Buscar usuario inexistente
- `ValidateCredentialsAsync_ShouldReturnUserForValidCredentials` - Validar credenciales correctas
- `ValidateCredentialsAsync_ShouldReturnNullForInvalidPassword` - Rechazar contraseña incorrecta
- `ValidateCredentialsAsync_ShouldReturnNullForNonexistentUser` - Rechazar usuario inexistente
- `AddAsync_ShouldHashPasswordAutomatically` - Hash automático de contraseñas

#### ControladorDeSesion (4 tests)
- `Register_ShouldReturnOkWithUserData_WhenRegistrationSucceeds` - Registro exitoso
- `Register_ShouldReturnConflict_WhenEmailAlreadyExists` - Rechazo de duplicados
- `Login_ShouldReturnTokenAndUserData_WhenCredentialsAreValid` - Login con credenciales válidas
- `Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid` - Rechazo de credenciales inválidas

### Ejecutar Tests

```bash
# Todos los tests
dotnet test

# Tests específicos
dotnet test --filter "InMemoryUsuarioRepository"
dotnet test --filter "ControladorDeSesion"

# Con cobertura
dotnet test /p:CollectCoverage=true
```

##  Endpoints API

### Autenticación

| Método | Endpoint | Autenticado | Descripción |
|--------|----------|-------------|-------------|
| POST | `/api/ControladorDeSesion/register` | No | Registrar nuevo usuario |
| POST | `/api/ControladorDeSesion/login` | No | Iniciar sesión y obtener JWT |
| GET | `/api/ControladorDeSesion/me` | Sí | Obtener datos del usuario autenticado |
| POST | `/api/ControladorDeSesion/logout` | No | Cerrar sesión (placeholder) |

### Ejemplos de Uso

**Registro**
```bash
curl -X POST http://localhost:5000/api/ControladorDeSesion/register \
  -H "Content-Type: application/json" \
  -d {
    "email": "usuario@example.com",
    "nombre": "Juan",
    "apellido": "Pérez",
    "contrasena": "MiContraseña123",
    "rol": "Estudiante",
    "edad": 25,
    "especializacion": "Programación"
  }
```

**Login**
```bash
curl -X POST http://localhost:5000/api/ControladorDeSesion/login \
  -H "Content-Type: application/json" \
  -d {
    "email": "usuario@example.com",
    "password": "MiContraseña123"
  }
```

**Acceso Protegido**
```bash
curl -X GET http://localhost:5000/api/ControladorDeSesion/me \
  -H "Authorization: Bearer <JWT_TOKEN>"
```

## Persistencia de Datos

### Opciones Disponibles

1. **InMemoryUsuarioRepository** (Actual)
   - Almacenamiento en memoria (se pierde al reiniciar)
   - Ideal para desarrollo y testing
   - Sin dependencias externas

2. **MongoUsuarioRepository** (Disponible)
   - Almacenamiento persistente en MongoDB
   - Ideal para producción
   - Requiere: `MongoDB.Driver`

### Cambiar a MongoDB

1. Editar `Program.cs`:
   ```csharp
   // Cambiar de:
   services.AddScoped<IUsuarioRepository, InMemoryUsuarioRepository>();
   
   // A:
   var mongoClient = new MongoClient("mongodb://localhost:27017");
   var database = mongoClient.GetDatabase("eduMentor");
   var usuariosCollection = database.GetCollection<Usuario>("Usuarios");
   services.AddScoped<IUsuarioRepository>(sp => new MongoUsuarioRepository(usuariosCollection));
   ```

2. Instalar MongoDB localmente o usar MongoDB Atlas (cloud)

## Frontend

### Estructura

- **HTML**: Páginas responsivas con Bootstrap 5 y Tailwind CSS
- **CSS**: Estilos personalizados y modernos
- **JavaScript**: Vanilla JS para interactividad, manejo de JWT

### Flujo de Usuario

1. **Inicio/Login** (`iniciodecesion.html`)
   - Formulario de login/registro
   - Guarda JWT en `localStorage.token`
   - Redirige a perfil tras autenticación

2. **Perfil** (`perfil.html`)
   - Obtiene datos del usuario desde `/me` usando JWT
   - Muestra historial de cursos, reservas, materiales
   - Fallback a `localStorage.currentUser` si no hay conexión

3. **Cursos** (`cursos.html`)
   - Visualización de cursos disponibles
   - Opciones de reserva y compra

## Ciclo de Desarrollo

### Flujo de Cambios

1. **Desarrollo Local**
   - `dotnet run` en TutoriasDeClases
   - Editar archivos HTML/CSS/JS en carpeta/
   - Actualizar modelos en Modelos/

2. **Testing**
   - `dotnet test` en TutoriasDeClases.Tests
   - Verificar 11/11 tests pasados

3. **Deployment**
   - Cambiar `appsettings.json` para producción
   - Usar MongoDB en lugar de InMemory
   - Cambiar claves JWT a valores seguros

## Mejoras Futuras

1. **Persistencia**: Migrar MongoDB con limpeza de datos inconsistentes
2. **Autenticación**: Implementar refresh tokens de larga duración
3. **Validación**: Agregar FluentValidation para validaciones complejas
4. **Logging**: Integrar Serilog para logging estructurado
5. **CI/CD**: Configurar GitHub Actions o Azure Pipelines
6. **Frontend**: Actualizar a framework moderno (React/Vue)
7. **Testing**: Agregar tests de integración y E2E
8. **API Docs**: Swagger/OpenAPI para documentación interactiva

##  Problemas Conocidos y Soluciones

### MongoDB con Datos Inconsistentes
- **Problema**: La base de datos MongoDB tenía usuarios con esquema inconsistente
- **Solución**: Usar `InMemoryUsuarioRepository` en desarrollo
- **Acción**: Antes de producción, limpiar datos de MongoDB o hacer migraciones

### JWT Expirado
- **Síntoma**: Error 401 en `/me`
- **Causa**: Token JWT expiró (120 minutos por defecto)
- **Solución**: Usuario debe volver a login para obtener token nuevo

### CORS
- **Configurado**: El backend permite CORS desde cualquier origen
- **En producción**: Restringir a dominios específicos en `Program.cs`

## Contacto y Soporte

1. Revisar esta documentación
2. Consultar los tests en `TutoriasDeClases.Tests/`
3. Verificar `Program.cs` para configuración de servicios

## Licencia

Proyecto educativo - 2024

---

**Última actualización**: Enero 202 
**Versión**: 1.0.0  
**Estado**: Funcional (Tests 11/11 pasados)
