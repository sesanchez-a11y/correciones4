# Diagrama UML - EduMentor

## Descripción General

Este diagrama UML representa la arquitectura completa del proyecto **EduMentor**, una plataforma de tutorías virtuales construida con:

- **Backend**: ASP.NET Core 9.0
- **Base de Datos**: MongoDB
- **Autenticación**: JWT (JSON Web Tokens)
- **Frontend**: HTML5, CSS3, JavaScript Vanilla

---

## 📊 Componentes Principales

### 1. **Modelos de Dominio**

#### `Usuario` (Clase Base Abstracta)
- Propiedades BSON para MongoDB
- Campos: nombre, apellido, correo, edad, especializacion, rol, contrasenaHash
- **Subclases**:
  - `Estudiante`: Hereda de Usuario
  - `Tutor`: Hereda de Usuario

#### `Servicio`
- Representa una clase o tutoría disponible
- Propiedades: id, titulo, precioBase

#### `Reserva`
- Modelo para gestionar reservas de tutorías
- Implementa **Patrón Observer** para notificaciones
- Contiene: id, servicio, alumnoId, total, observadores
- Métodos: AgregarObservador(), Notificar(), Confirmar()

#### `Pago`
- Gestión de pagos
- Utiliza **Patrón Strategy** (IPago)
- Propiedades: id, monto, procesador

#### `Notificacion`
- Modelo para almacenar mensajes de notificación

---

### 2. **Interfaces (Contratos)**

#### `IUsuarioRepository`
```csharp
Task<string> AddAsync(Usuario user)
Task<Usuario?> FindByEmailAsync(string email)
Task<Usuario?> ValidateCredentialsAsync(string email, string password)
```
**Patrón**: Repository Pattern
- Abstrae la lógica de acceso a datos
- Permite cambiar implementaciones sin afectar controladores

#### `INotificacion`
```csharp
void Enviar(string mensaje)
```
**Patrón**: Observer Pattern
- Define contrato para notificadores
- Permite múltiples implementaciones (Email, SMS, Push)

#### `IPago`
```csharp
bool Procesar(double monto)
```
- Contrato para procesadores de pago

#### `IPrecioStrategy`
```csharp
double Calcular(double precioBase)
```
**Patrón**: Strategy Pattern
- Diferentes estrategias de cálculo de precios

---

### 3. **Implementaciones de Repositorio**

#### `InMemoryUsuarioRepository`
- Almacena usuarios en memoria (Desarrollo)
- Útil para testing
- No persiste datos entre sesiones

#### `MongoUsuarioRepository`
- Persiste usuarios en MongoDB (Producción)
- Usa MongoDB.Driver
- Incluye validación con BCrypt

---

### 4. **Observadores (Notificaciones)**

#### `EmailNotificacion`
- Implementa INotificacion
- Envía notificaciones por email

#### `SmsNotificacion`
- Implementa INotificacion
- Envía notificaciones por SMS

---

### 5. **Estrategias de Precio**

#### `PrecioBase`
- Calcula precio sin modificaciones

#### `PrecioConDescuento`
- Aplica descuento porcentual
- Parámetro: porcentajeDescuento

#### `PrecioConImpuesto`
- Aplica impuesto porcentual
- Parámetro: porcentajeImpuesto

---

### 6. **Factory**

#### `ReservaFactory`
- Patrón Factory Method
- Simplifica creación de objetos Reserva
- Método: CrearReserva(alumnoId, servicio)

---

### 7. **Controladores**

#### `ControladorDeSesion`
```
POST   /api/ControladorDeSesion/register  → Registro de usuario
POST   /api/ControladorDeSesion/login     → Login y generación JWT
GET    /api/ControladorDeSesion/me        → Validar JWT [Authorize]
GET    /api/ControladorDeSesion/test      → Test endpoint
```

**Responsabilidades**:
- Autenticación (registro/login)
- Generación y validación de JWT
- Manejo de errores (Conflict 409, BadRequest 400, etc)

#### `ReservasController`
- Gestión de reservas
- CRUD de reservas

#### `DebugController`
- Endpoint para debugging
- GET `/api/debug/userByEmail` - Busca usuario por email

---

### 8. **Modelos de Solicitud**

#### `RegistroModel`
- Datos del formulario de registro
- Campos: rol, email, nombre, apellido, edad, especializacion, contrasena

#### `LoginModel`
- Datos del formulario de login
- Campos: email, password/contrasena

---

## 🏗️ Patrones de Diseño Implementados

| Patrón | Dónde | Beneficio |
|--------|-------|----------|
| **Repository** | IUsuarioRepository + InMemoryUsuarioRepository + MongoUsuarioRepository | Desacoplamiento de acceso a datos |
| **Dependency Injection** | Program.cs (DI Container) | Flexibilidad y testabilidad |
| **Observer** | Reserva + INotificacion + EmailNotificacion + SmsNotificacion | Notificaciones desacopladas |
| **Strategy** | IPrecioStrategy + PrecioBase + PrecioConDescuento + PrecioConImpuesto | Cálculos de precio intercambiables |
| **Factory** | ReservaFactory | Creación simplificada de Reservas |
| **JWT Authentication** | ControladorDeSesion + [Authorize] | Seguridad sin estado de sesión |

---

## 🔄 Flujos Principales

### 1. **Flujo de Registro**
```
Usuario (Frontend)
    ↓
POST /register (RegistroModel)
    ↓
ControladorDeSesion.Register()
    ↓
Validar no existe (IUsuarioRepository)
    ↓
Crear Usuario (Estudiante/Tutor)
    ↓
Hash contraseña (BCrypt)
    ↓
AddAsync(usuario)
    ↓
MongoUsuarioRepository → MongoDB
    ↓
Retorna usuario registrado
```

### 2. **Flujo de Login**
```
Usuario (Frontend)
    ↓
POST /login (LoginModel)
    ↓
ControladorDeSesion.Login()
    ↓
ValidateCredentialsAsync(email, password)
    ↓
Comparar BCrypt hash
    ↓
Si válido: Generar JWT
    ↓
Retorna token + datos usuario
```

### 3. **Flujo de Validación (JWT)**
```
Cliente envía: GET /me
Header: Authorization: Bearer <token>
    ↓
Middleware JWT valida token
    ↓
[Authorize] permite acceso
    ↓
ControladorDeSesion.Me()
    ↓
Retorna datos del usuario autenticado
```

---

## 🗄️ Estructura de MongoDB

**Base de Datos**: `EduMentor`

### Colección: `Usuarios`
```json
{
  "_id": ObjectId,
  "nombre": "Juan",
  "apellido": "Pérez",
  "correo": "juan@example.com",
  "edad": 25,
  "especializacion": "Desarrollo Web",
  "rol": "Alumno",
  "contrasenaHash": "$2b$..."
}
```

---

## 🔐 Seguridad Implementada

1. **Hashing de Contraseñas**: BCrypt.Net-Next
2. **JWT**: HS256, configurado en `Program.cs`
3. **CORS**: Configurado para localhost:5000
4. **[Authorize]**: Atributo en endpoints sensibles
5. **Validación de entrada**: En modelos RegistroModel y LoginModel

---

## 📦 Dependencias del Proyecto

```xml
<PackageReference Include="MongoDB.Driver" Version="3.5.0" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.2" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
```

---

## 🧪 Testing

El proyecto incluye tests unitarios con **xUnit**:

- `InMemoryUsuarioRepositoryTests.cs` - 7 tests
- `ControladorDeSesionTests.cs` - 4 tests

**Total**: 11 tests (100% pasadas ✅)

---

## 📋 Resumen Ejecutivo

**EduMentor** es una plataforma modular y escalable que:

✅ Implementa **SOLID principles**  
✅ Usa **patrones de diseño** reconocidos  
✅ Separación clara entre **capas** (Modelos, Interfaces, Repositorio, Controladores)  
✅ Autenticación **segura** con JWT  
✅ **Persistencia** en MongoDB  
✅ **Tests unitarios** con buena cobertura  
✅ **Frontend interactivo** con validación en tiempo real  

---

**Generado**: 2 de diciembre de 2025  
**Versión Backend**: ASP.NET Core 9.0  
**Versión Base de Datos**: MongoDB 5.0+
