# 📋 DIAGRAMAS UML - EduMentor

## 📑 Indice de Documentos

Este conjunto de documentos proporciona una visión completa de la arquitectura de **EduMentor**, una plataforma de tutorías virtuales.

### Archivos Generados

1. **`DIAGRAMA_UML.puml`** - Diagrama de Clases (Class Diagram)
2. **`DIAGRAMA_SECUENCIA_AUTH.puml`** - Diagrama de Secuencia de Autenticación
3. **`DIAGRAMA_CAPAS.puml`** - Diagrama de Capas Arquitectónicas
4. **`DIAGRAMA_CASOS_USO.puml`** - Diagrama de Casos de Uso
5. **`DOCUMENTACION_UML.md`** - Documentación Detallada
6. **`RESUMEN_DIAGRAMAS.md`** - Este archivo

---

## 🎯 Descripción de cada Diagrama

### 1. **DIAGRAMA_UML.puml** - Diagrama de Clases

**Propósito**: Mostrar la estructura estática del proyecto.

**Componentes**:
- ✅ Modelos de dominio (Usuario, Estudiante, Tutor, Servicio, Reserva, Pago)
- ✅ Interfaces (IUsuarioRepository, INotificacion, IPago, IPrecioStrategy)
- ✅ Implementaciones (InMemoryUsuarioRepository, MongoUsuarioRepository, EmailNotificacion, SmsNotificacion)
- ✅ Estrategias de precio (PrecioBase, PrecioConDescuento, PrecioConImpuesto)
- ✅ Controladores (ControladorDeSesion, ReservasController, DebugController)
- ✅ Factory (ReservaFactory)

**Patrones Visualizados**:
- Repository Pattern
- Observer Pattern
- Strategy Pattern
- Factory Pattern
- Dependency Injection

**Cómo usarlo**:
```bash
# Para renderizar a PNG/SVG, usa PlantUML Online o instala plantuml localmente
plantuml DIAGRAMA_UML.puml -Tpng -o DIAGRAMA_UML.png
```

---

### 2. **DIAGRAMA_SECUENCIA_AUTH.puml** - Secuencia de Autenticación

**Propósito**: Mostrar flujos temporales de autenticación.

**Flujos Cubiertos**:

#### 📝 Registro
1. Cliente completa formulario
2. Frontend valida campos
3. API recibe POST /register
4. Repository verifica email único
5. Crea usuario (Estudiante/Tutor)
6. Hash BCrypt contraseña
7. Inserta en MongoDB
8. Genera JWT token
9. Retorna token + usuario
10. Frontend guarda en localStorage
11. Redirige a perfil.html

#### 🔑 Validación JWT
1. Frontend lee token de localStorage
2. Envía GET /me con Bearer token
3. Middleware JWT valida
4. [Authorize] verifica claims
5. Obtiene datos usuario de MongoDB
6. Retorna información al cliente
7. Frontend actualiza DOM con perfil

#### 🔓 Login
1. Cliente ingresa credenciales
2. Frontend valida
3. API recibe POST /login
4. Repository valida con BCrypt
5. Genera JWT
6. Retorna token + datos
7. Frontend guarda y redirige

#### 🚪 Logout
1. Usuario clickea botón logout
2. Sistema muestra confirmación
3. Elimina token de localStorage
4. Elimina datos usuario de localStorage
5. Redirige a inicio.html

---

### 3. **DIAGRAMA_CAPAS.puml** - Arquitectura de Capas

**Propósito**: Mostrar la división de responsabilidades en capas.

**Capas**:

#### 🖥️ Capa de Presentación (Frontend)
- HTML5 (estructura)
- CSS3 (estilos)
- JavaScript Vanilla (lógica cliente)
- Bootstrap 5 (componentes)
- Tailwind CSS (utilities)

#### 🌐 Capa de API (ASP.NET Core)
- Controllers (ControladorDeSesion, ReservasController, DebugController)
- Middleware (JWT Auth, CORS)
- Servicios (Autenticación, Notificaciones, Pagos)

#### 📊 Capa de Modelos y Lógica
- Modelos de Dominio (Usuario, Reserva, etc)
- Interfaces (IUsuarioRepository, INotificacion, etc)
- Patrones (Repository, Observer, Strategy, Factory)

#### 💾 Capa de Datos
- Repositorios (InMemory para desarrollo, Mongo para producción)
- MongoDB (Base de datos NoSQL)
- Colecciones (Usuarios, Reservas, Pagos, Servicios)

#### 🔐 Capa de Seguridad
- Autenticación (BCrypt + JWT)
- Validación de entrada
- [Authorize] middleware

---

### 4. **DIAGRAMA_CASOS_USO.puml** - Casos de Uso

**Propósito**: Mostrar funcionalidades desde la perspectiva del usuario.

**Actores**:
- 👤 Visitante (no registrado)
- 👨‍🎓 Estudiante
- 👨‍🏫 Tutor
- ⚙️ Admin

**Casos de Uso** (19 total):

| # | Caso de Uso | Actores | Descripción |
|---|---|---|---|
| 1 | Registrarse | Visitante | Crear nueva cuenta |
| 2 | Iniciar Sesión | Visitante, Estudiante, Tutor | Autenticarse |
| 3 | Ver Perfil | Estudiante, Tutor | Consultar datos personales |
| 4 | Cerrar Sesión | Estudiante, Tutor | Logout |
| 5 | Editar Perfil | Estudiante, Tutor | Modificar datos |
| 6 | Ver Tutores Disponibles | Visitante, Estudiante | Listar tutores |
| 7 | Buscar Tutorías | Estudiante | Filtrar por criterios |
| 8 | Reservar Tutoría | Estudiante | Crear reserva |
| 9 | Cancelar Reserva | Estudiante | Cancelar reserva |
| 10 | Procesar Pago | Estudiante | Pagar tutoría |
| 11 | Ver Historial | Estudiante, Tutor | Clases pasadas |
| 12 | Calificar | Estudiante | Evaluar tutoría |
| 13 | Ver Estudiantes | Tutor | Mis estudiantes |
| 14 | Crear Disponibilidad | Tutor | Horarios disponibles |
| 15 | Aceptar/Rechazar Reserva | Tutor | Gestionar solicitudes |
| 16 | Recibir Notificación | Estudiante, Tutor | Email/SMS alerts |
| 17 | Ver Estadísticas | Admin | Métricas del sistema |
| 18 | Gestionar Usuarios | Admin | CRUD de usuarios |
| 19 | Monitorear Pagos | Admin | Transacciones |

---

## 🔄 Flujos de Datos

### Flujo de Registro → Login → Perfil

```
┌─────────────────┐
│  VISITANTE      │
└────────┬────────┘
         │
         ↓ Completa formulario
    ┌────────────────────────────────┐
    │  POST /register                │
    │  {email, password, rol}        │
    └────────┬───────────────────────┘
             │
             ↓ Valida + Hash BCrypt
    ┌────────────────────────────────┐
    │  MongoDB INSERT                │
    │  collection: Usuarios          │
    └────────┬───────────────────────┘
             │
             ↓ Genera JWT
    ┌────────────────────────────────┐
    │  Retorna {token, user}         │
    │  localStorage.setItem(token)   │
    └────────┬───────────────────────┘
             │
             ↓ Redirige
    ┌────────────────────────────────┐
    │  GET perfil.html               │
    │  GET /me (validar JWT)         │
    └────────┬───────────────────────┘
             │
             ↓ Actualiza DOM
    ┌────────────────────────────────┐
    │  ESTUDIANTE AUTENTICADO        │
    │  Perfil cargado                │
    └────────────────────────────────┘
```

---

## 🏗️ Patrones de Diseño Aplicados

| Patrón | Componentes | Beneficio |
|--------|-------------|----------|
| **Repository** | IUsuarioRepository + InMemory + Mongo | Abstracción de datos, fácil de testear |
| **Dependency Injection** | Program.cs | Desacoplamiento, inyección de dependencias |
| **Observer** | Reserva + INotificacion | Desacoplamiento de notificadores |
| **Strategy** | IPrecioStrategy + implementaciones | Algoritmos intercambiables |
| **Factory** | ReservaFactory | Creación simplificada |
| **JWT Auth** | ControladorDeSesion | Autenticación stateless y segura |
| **DTO** | RegistroModel, LoginModel | Mapeo de datos entrada/salida |

---

## 📊 Estadísticas del Proyecto

- **Modelos de Dominio**: 7 clases
- **Interfaces**: 4 contratos
- **Controladores**: 3 controladores
- **Repositorios**: 2 implementaciones
- **Observadores**: 2 (Email, SMS)
- **Estrategias de Precio**: 3
- **Endpoints API**: 6+ 
- **Tests Unitarios**: 11 (100% passing)
- **Líneas de Código**: ~2000+ (sin frontend)

---

## 🚀 Instrucciones para Visualizar

### Opción 1: PlantUML Online
1. Ve a https://www.plantuml.com/plantuml/uml/
2. Copia el contenido de cualquier archivo `.puml`
3. Pega en el editor online
4. Visualiza automáticamente

### Opción 2: Instalar PlantUML Localmente

```bash
# Windows (con Chocolatey)
choco install plantuml

# O descargar JAR directamente
java -jar plantuml.jar DIAGRAMA_UML.puml -Tpng
```

### Opción 3: VS Code Extension
1. Instala "PlantUML" extension
2. Abre archivo `.puml`
3. Presiona `Alt+D` para preview

---

## 📈 Próximos Pasos Sugeridos

1. **Validación de Diagrama**
   - Revisar que los diagramas reflejan la arquitectura actual
   - Ajustar si hay cambios pendientes

2. **Generación de Imágenes**
   - Exportar diagramas a PNG/SVG
   - Incluir en documentación del proyecto

3. **Documentación de Desarrollo**
   - Crear guía de extensión (cómo agregar nuevo controlador)
   - Documentar API endpoints con OpenAPI/Swagger

4. **Testing**
   - Aumentar cobertura de tests
   - Agregar tests de integración

5. **Deployment**
   - Preparar configuración para producción
   - Documento de deployment a servidor

---

## ✅ Checklist de Documentación

- [x] Diagrama de Clases (UML)
- [x] Diagrama de Secuencia (Autenticación)
- [x] Diagrama de Capas (Arquitectura)
- [x] Diagrama de Casos de Uso
- [x] Documentación Detallada (DOCUMENTACION_UML.md)
- [x] Resumen Ejecutivo (este archivo)
- [ ] Exportar a imágenes PNG/SVG
- [ ] Agregar a README principal
- [ ] Documentación API (OpenAPI)
- [ ] Guía de desarrollo

---

## 📞 Información de Contacto

**Proyecto**: EduMentor - Plataforma de Tutorías Virtuales  
**Versión**: 1.0  
**Fecha**: 2 de diciembre de 2025  
**Stack**: ASP.NET Core 9.0 + MongoDB + JavaScript Vanilla  
**Tests**: xUnit (11 tests, 100% passing)

---

## 📚 Referencias

- [PlantUML Documentation](https://plantuml.com/)
- [ASP.NET Core Docs](https://docs.microsoft.com/dotnet/core/)
- [MongoDB Docs](https://docs.mongodb.com/)
- [Design Patterns](https://refactoring.guru/design-patterns)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)

---

**Generado automáticamente** | **Usa PlantUML Online para visualizar**
