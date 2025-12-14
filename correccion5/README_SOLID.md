# Documentación de Principios SOLID - EduMentor

Este documento explica cómo se han aplicado los principios SOLID en el proyecto EduMentor, cumpliendo con los requisitos de diseño y arquitectura de software.

## Índice
1. [Arquitectura del Proyecto](#arquitectura-del-proyecto)
2. [Aplicación de SOLID](#aplicación-de-solid)
3. [Ejemplos Concretos](#ejemplos-concretos)
4. [Pruebas y Calidad](#pruebas-y-calidad)

---

## Arquitectura del Proyecto

El proyecto está organizado en capas que separan responsabilidades:

```
TutoriasDeClasesbackend/
├── Modelos/           # Entidades de dominio (Curso, Usuario, Reserva, etc.)
├── Interfaces/        # Abstracciones (ICursoRepository, ICursoService, etc.)
├── Servicios/         # Lógica de negocio (CursoService)
├── Repositorios/      # Acceso a datos (CursoRepository, MongoUsuarioRepository)
├── Controllers/       # API REST endpoints
├── Factories/         # Patrones Factory
├── Observers/         # Patrón Observer para notificaciones
└── Strategies/        # Patrón Strategy para precios
```

---

## Aplicación de SOLID

### 1. S - Single Responsibility Principle (SRP)

**Definición**: Cada clase debe tener una única responsabilidad y razón para cambiar.

#### ✅ Ejemplo 1: Separación de Servicios y Repositorios

**Antes (Violación de SRP):**
```csharp
// ❌ Controller hacía TODO: validación, lógica de negocio y persistencia
public class CursosController {
    public async Task<IActionResult> CrearCurso(CrearCursoDto dto) {
        // Validación en el controller
        if (string.IsNullOrEmpty(dto.Nombre)) return BadRequest();
        
        // Lógica de negocio en el controller
        var curso = new Curso { /* ... */ };
        
        // Persistencia directa en el controller
        await _cursoRepo.AddAsync(curso);
    }
}
```

**Después (Cumple SRP):**
```csharp
// ✅ SEPARACIÓN DE RESPONSABILIDADES

// 1. Modelo de Dominio - Solo representa la entidad
public class Curso {
    public string Id { get; set; }
    public string Nombre { get; set; }
    public int Capacidad { get; set; }
    public List<string> Estudiantes { get; set; }
}

// 2. Repositorio - Solo maneja persistencia de datos
public class CursoRepository : ICursoRepository {
    public async Task<string> AddAsync(Curso curso) { /* ... */ }
    public async Task<Curso> GetByIdAsync(string id) { /* ... */ }
}

// 3. Servicio - Solo maneja lógica de negocio
public class CursoService : ICursoService {
    public async Task<(bool, string, string)> CrearCursoAsync(...) {
        // Validación de reglas de negocio
        var validacion = ValidarDatosCurso(...);
        if (!validacion.IsValid) return (false, validacion.ErrorMessage, "");
        
        // Creación del curso
        var curso = new Curso { /* ... */ };
        var id = await _cursoRepo.AddAsync(curso);
        return (true, "Curso creado", id);
    }
}

// 4. Controller - Solo maneja HTTP y orquestación
public class CursosController : ControllerBase {
    private readonly ICursoService _cursoService;
    
    public async Task<IActionResult> CrearCurso(CrearCursoDto dto) {
        var resultado = await _cursoService.CrearCursoAsync(...);
        return resultado.Success ? Ok(resultado) : BadRequest(resultado);
    }
}
```

**Beneficio**: Cada clase tiene UNA responsabilidad clara. Los cambios en la lógica de negocio no afectan al repositorio ni al controller.

---

### 2. O - Open/Closed Principle (OCP)

**Definición**: Las clases deben estar abiertas para extensión pero cerradas para modificación.

#### ✅ Ejemplo 2: Patrón Strategy para Precios

**Implementación:**
```csharp
// Interfaz que permite extender sin modificar código existente
public interface IPrecioStrategy {
    double CalcularPrecio(double precioBase);
}

// Diferentes estrategias de precio (extensión)
public class PrecioNormalStrategy : IPrecioStrategy {
    public double CalcularPrecio(double precioBase) => precioBase;
}

public class PrecioDescuentoStrategy : IPrecioStrategy {
    private readonly double _descuento;
    public PrecioDescuentoStrategy(double descuento) => _descuento = descuento;
    public double CalcularPrecio(double precioBase) => precioBase * (1 - _descuento);
}

public class PrecioPremiumStrategy : IPrecioStrategy {
    public double CalcularPrecio(double precioBase) => precioBase * 1.5;
}

// Uso - Se pueden añadir nuevas estrategias sin modificar código existente
public class Servicio {
    private IPrecioStrategy _precioStrategy;
    
    public void SetPrecioStrategy(IPrecioStrategy strategy) {
        _precioStrategy = strategy;
    }
    
    public double ObtenerPrecioFinal() {
        return _precioStrategy.CalcularPrecio(PrecioBase);
    }
}
```

**Beneficio**: Podemos añadir nuevas estrategias de precio (Black Friday, Estudiantes, etc.) sin modificar las clases existentes.

---

### 3. L - Liskov Substitution Principle (LSP)

**Definición**: Las clases derivadas deben poder sustituir a sus clases base sin alterar el comportamiento del programa.

#### ✅ Ejemplo 3: Jerarquía de Usuarios

**Implementación:**
```csharp
// Clase base
public class Usuario {
    public string Id { get; set; }
    public string Nombre { get; set; }
    public string Correo { get; set; }
    public string Rol { get; set; }
}

// Clases derivadas que pueden sustituir a Usuario
public class Tutor : Usuario {
    // Añade funcionalidad específica sin romper comportamiento base
    public List<string> Especialidades { get; set; }
}

public class Estudiante : Usuario {
    // Añade funcionalidad específica sin romper comportamiento base
    public List<string> CursosInscritos { get; set; }
}

// LSP en acción - cualquier Usuario puede usarse
public async Task<Usuario> ObtenerUsuario(string id) {
    // Puede retornar Usuario, Tutor o Estudiante sin problemas
    var usuario = await _usuarioRepo.GetByIdAsync(id);
    return usuario; // Funciona con cualquier subtipo
}
```

**Beneficio**: Cualquier parte del código que espere un `Usuario` puede recibir un `Tutor` o `Estudiante` sin problemas.

---

### 4. I - Interface Segregation Principle (ISP)

**Definición**: Los clientes no deben depender de interfaces que no usan. Es mejor tener muchas interfaces específicas que una general.

#### ✅ Ejemplo 4: Interfaces Segregadas

**Antes (Violación de ISP):**
```csharp
// ❌ Interfaz muy grande - no todos los clientes necesitan todos los métodos
public interface IRepositorio {
    Task<T> GetByIdAsync(string id);
    Task<List<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(string id);
    Task<List<T>> SearchByNameAsync(string name);
    Task<List<T>> GetByDateAsync(DateTime date);
    Task<int> CountAsync();
}
```

**Después (Cumple ISP):**
```csharp
// ✅ Interfaces específicas según necesidad

public interface ICursoRepository {
    Task<string> AddAsync(Curso curso);
    Task<Curso> GetByIdAsync(string id);
    Task<List<Curso>> GetAllAsync();
    Task<List<Curso>> GetByTutorIdAsync(string tutorId);
    Task<List<Curso>> GetAprobadosAsync();
    Task UpdateAsync(Curso curso);
    Task DeleteAsync(string id);
}

public interface ICursoService {
    Task<(bool Success, string Message, string CursoId)> CrearCursoAsync(...);
    Task<(bool Success, string Message)> InscribirEstudianteAsync(string cursoId, string estudianteId);
    Task<bool> TieneCuposDisponiblesAsync(string cursoId);
    (bool IsValid, string ErrorMessage) ValidarDatosCurso(...);
}

public interface INotificacion {
    void Enviar(string mensaje);
}
```

**Beneficio**: Cada interfaz contiene solo los métodos relevantes para su propósito. Los clientes no dependen de métodos que no necesitan.

---

### 5. D - Dependency Inversion Principle (DIP)

**Definición**: Las clases de alto nivel no deben depender de clases de bajo nivel. Ambas deben depender de abstracciones.

#### ✅ Ejemplo 5: Inyección de Dependencias

**Antes (Violación de DIP):**
```csharp
// ❌ Dependencia directa de implementación concreta
public class CursosController {
    private readonly CursoRepository _repo; // Clase concreta
    
    public CursosController() {
        _repo = new CursoRepository(); // Creación directa
    }
}
```

**Después (Cumple DIP):**
```csharp
// ✅ Dependencia de abstracción

// 1. Definir abstracción (interface)
public interface ICursoService {
    Task<(bool, string, string)> CrearCursoAsync(...);
}

public interface ICursoRepository {
    Task<string> AddAsync(Curso curso);
}

// 2. Implementaciones concretas dependen de abstracciones
public class CursoService : ICursoService {
    private readonly ICursoRepository _cursoRepo; // Abstracción
    private readonly IUsuarioRepository _usuarioRepo; // Abstracción
    
    public CursoService(ICursoRepository cursoRepo, IUsuarioRepository usuarioRepo) {
        _cursoRepo = cursoRepo;
        _usuarioRepo = usuarioRepo;
    }
}

// 3. Controller depende de abstracción
public class CursosController : ControllerBase {
    private readonly ICursoService _cursoService; // Abstracción
    
    public CursosController(ICursoService cursoService) {
        _cursoService = cursoService;
    }
}

// 4. Configuración en Program.cs - Inversión de Control
builder.Services.AddScoped<ICursoRepository, CursoRepository>();
builder.Services.AddScoped<ICursoService, CursoService>();
```

**Beneficio**: 
- Fácil cambiar implementaciones (ej: de MongoDB a SQL Server)
- Facilita testing con mocks
- Bajo acoplamiento entre componentes

---

## Ejemplos Concretos

### Resumen de Aplicación de SOLID en el Proyecto

| Principio | Ubicación | Descripción |
|-----------|-----------|-------------|
| **SRP** | `TutoriasDeClasesbackend/Servicios/CursoService.cs` | Servicio dedicado exclusivamente a lógica de negocio de cursos |
| **SRP** | `TutoriasDeClasesbackend/Repositorios/CursoRepository.cs` | Repositorio dedicado exclusivamente a persistencia de cursos |
| **OCP** | `TutoriasDeClasesbackend/Strategies/` | Estrategias de precio extensibles sin modificar código |
| **LSP** | `TutoriasDeClasesbackend/Modelos/Usuario.cs` | Jerarquía Usuario → Tutor, Estudiante sustituibles |
| **ISP** | `TutoriasDeClasesbackend/Interfaces/ICursoService.cs` | Interfaces segregadas por responsabilidad |
| **ISP** | `TutoriasDeClasesbackend/Interfaces/INotificacion.cs` | Interfaz simple para notificaciones |
| **DIP** | `TutoriasDeClasesbackend/Program.cs` | Inyección de dependencias con abstracciones |

---

## Pruebas y Calidad

### Pruebas Unitarias

Ubicación: `TutoriasDeClases.Tests/`

Las pruebas verifican la lógica de negocio aisladamente gracias a SOLID:

```csharp
[Fact]
public async Task InscribirEstudianteAsync_ConCursoLleno_DebeRetornarError() {
    // Arrange - Mock de dependencias (gracias a DIP)
    var mockCursoRepo = new Mock<ICursoRepository>();
    var curso = new Curso { Capacidad = 2, Estudiantes = new List<string> { "e1", "e2" } };
    mockCursoRepo.Setup(r => r.GetByIdAsync("curso123")).ReturnsAsync(curso);
    
    var service = new CursoService(mockCursoRepo.Object, Mock.Of<IUsuarioRepository>());
    
    // Act
    var resultado = await service.InscribirEstudianteAsync("curso123", "e3");
    
    // Assert
    Assert.False(resultado.Success);
    Assert.Contains("lleno", resultado.Message.ToLower());
}
```

**Pruebas implementadas:**
- ✅ Creación y validación de cursos
- ✅ Inscripción de estudiantes (éxito y errores)
- ✅ Validación de cupos disponibles
- ✅ Reglas de negocio (curso lleno, duplicados, estados)
- ✅ Aprobación y rechazo de cursos

### Pruebas de Rendimiento

Ubicación: `k6-tests/`

Ejecutar: `k6 run k6-tests/load-test.js`

**Configuración:**
- 10 usuarios virtuales concurrentes
- 30 segundos de duración
- Endpoints: `/api/ping` y `/api/cursos/aprobados`

**Resultados esperados:**
- P95 < 500ms
- Tasa de error < 5%
- ~14 req/s con 10 usuarios

---

## Beneficios de Aplicar SOLID

1. **Mantenibilidad**: Cambios en una parte no afectan otras
2. **Testabilidad**: Fácil crear pruebas unitarias con mocks
3. **Escalabilidad**: Fácil añadir nuevas funcionalidades
4. **Legibilidad**: Código más claro y organizado
5. **Reutilización**: Componentes desacoplados reutilizables

---

## Conclusión

El proyecto EduMentor cumple con los principios SOLID mediante:

✅ **SRP**: Separación clara de responsabilidades (Servicios, Repositorios, Controllers)  
✅ **OCP**: Patrones extensibles (Strategy, Factory)  
✅ **LSP**: Jerarquías correctas de herencia (Usuario)  
✅ **ISP**: Interfaces específicas y segregadas  
✅ **DIP**: Inyección de dependencias con abstracciones  

Además, cuenta con:
✅ Pruebas unitarias completas de lógica de negocio  
✅ Pruebas de rendimiento con k6  
✅ Documentación detallada de implementación  

---

**Autor**: Sistema EduMentor  
**Fecha**: Diciembre 2024  
**Versión**: 1.0
