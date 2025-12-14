# 📖 GUÍA RÁPIDA - DIAGRAMAS UML DEL PROYECTO

Bienvenido a la documentación visual de **EduMentor**. Los diagramas UML ayudan a entender rápidamente cómo funciona la aplicación.

---

## 📊 Archivos de Diagramas (PlantUML)

### 1. 🏛️ **DIAGRAMA_UML.puml**
Muestra la estructura estática: clases, interfaces, relaciones.

**Ver online**:
- Ve a https://www.plantuml.com/plantuml/uml/
- Copia-pega el contenido del archivo

**Lo que encontrarás**:
- ✅ Herencia (Usuario → Estudiante/Tutor)
- ✅ Interfaces (IUsuarioRepository, INotificacion, etc)
- ✅ Composición (Reserva contiene Servicio)
- ✅ Implementaciones (InMemoryUsuarioRepository, MongoUsuarioRepository)
- ✅ Patrones de diseño (Repository, Observer, Strategy, Factory)

---

### 2. 🔐 **DIAGRAMA_SECUENCIA_AUTH.puml**
Muestra cómo fluye la autenticación paso a paso.

**Flujos incluidos**:
1. **Registro** (Visitante → Usuario registrado)
2. **Validación JWT** (Enviar token → Obtener datos)
3. **Login** (Credenciales → JWT token)
4. **Logout** (Limpiar localStorage)

**Útil para**: Entender timing y orden de operaciones

---

### 3. 🏗️ **DIAGRAMA_CAPAS.puml**
Muestra la arquitectura en capas del sistema.

**Capas**:
```
┌─────────────────────────────┐
│  🖥️ PRESENTACIÓN (Frontend)  │  HTML, CSS, JS, Bootstrap, Tailwind
├─────────────────────────────┤
│  🌐 API (ASP.NET Core)      │  Controllers, Middleware, Servicios
├─────────────────────────────┤
│  📊 MODELOS Y LÓGICA       │  Clases, Interfaces, Patrones
├─────────────────────────────┤
│  💾 DATOS (MongoDB)         │  Repositorios, Collections
├─────────────────────────────┤
│  🔐 SEGURIDAD               │  BCrypt, JWT, [Authorize]
└─────────────────────────────┘
```

---

### 4. 👥 **DIAGRAMA_CASOS_USO.puml**
Muestra qué puede hacer cada tipo de usuario.

**Actores**:
- 👤 **Visitante**: Registrarse, ver tutores
- 👨‍🎓 **Estudiante**: Buscar, reservar, pagar tutorías
- 👨‍🏫 **Tutor**: Gestionar disponibilidad, aceptar reservas
- ⚙️ **Admin**: Estadísticas, gestionar usuarios

---

## 📚 Documentación Completa

### **DOCUMENTACION_UML.md**
Guía detallada que explica:
- Cada clase y su responsabilidad
- Patrones implementados
- Flujos principales
- Estructura de MongoDB
- Seguridad implementada

### **RESUMEN_DIAGRAMAS.md**
Índice completo con:
- Descripción de cada diagrama
- Flujos de datos
- Estadísticas del proyecto
- Checklist de documentación

---

## 🎯 Dónde Empezar

### Si quieres entender...

**"¿Cómo se registra un usuario?"**
→ Ver: **DIAGRAMA_SECUENCIA_AUTH.puml** (Sección Registro)

**"¿Qué clases existen en el backend?"**
→ Ver: **DIAGRAMA_UML.puml** (Sección Modelos de Dominio)

**"¿Cómo se comunican Frontend y Backend?"**
→ Ver: **DIAGRAMA_CAPAS.puml** (Relaciones entre capas)

**"¿Qué puede hacer un Tutor?"**
→ Ver: **DIAGRAMA_CASOS_USO.puml** (Actor: Tutor)

**"¿Qué patrones de diseño se usan?"**
→ Ver: **DOCUMENTACION_UML.md** (Sección Patrones)

---

## 🚀 Cómo Visualizar

### Opción 1️⃣ : Online (Sin instalar nada)
```
1. Ve a https://www.plantuml.com/plantuml/uml/
2. Copiar contenido del archivo .puml
3. Pegar en la caja de texto
4. Ver resultado automáticamente
```

### Opción 2️⃣ : VS Code Extension
```
1. Instala "PlantUML" (jebbs.plantuml)
2. Abre archivo .puml
3. Presiona Alt+D para preview
```

### Opción 3️⃣ : Línea de comandos
```bash
# Si tienes Java instalado
java -jar plantuml.jar DIAGRAMA_UML.puml -Tpng

# Si tienes npm
npm install -g plantuml
plantuml DIAGRAMA_UML.puml
```

---

## 📋 Lista de Archivos

```
.\
├── DIAGRAMA_UML.puml              ← Diagrama de clases
├── DIAGRAMA_SECUENCIA_AUTH.puml   ← Flujos de autenticación
├── DIAGRAMA_CAPAS.puml            ← Arquitectura en capas
├── DIAGRAMA_CASOS_USO.puml        ← Funcionalidades por usuario
├── DOCUMENTACION_UML.md           ← Documentación detallada
├── RESUMEN_DIAGRAMAS.md           ← Índice de diagramas
├── GUIA_RAPIDA_UML.md             ← Este archivo
└── README.md                      ← Documentación principal
```

---

## 🔍 Elementos Comunes en UML

| Símbolo | Significado | Ejemplo |
|---------|-------------|---------|
| `→` | Relación simple | Usuario → Cuenta |
| `---->` | Herencia | Estudiante hereda de Usuario |
| `◇--` | Composición (parte de) | Reserva contiene Servicio |
| `*--` | Agregación | Usuario tiene múltiples Reservas |
| `.>` | Dependencia | Controlador depende de Repository |
| `<<interface>>` | Interface | IUsuarioRepository |
| `<<abstract>>` | Clase abstracta | Usuario |
| `+` | Público | `+ getUser()` |
| `-` | Privado | `- password` |
| `#` | Protegido | `# data` |

---

## ✨ Características del Proyecto Visualizadas

✅ **SOLID Principles**
- Single Responsibility: Cada clase tiene una responsabilidad
- Open/Closed: Interfaces abiertas para extensión
- Liskov Substitution: Implementaciones intercambiables
- Interface Segregation: Interfaces específicas
- Dependency Inversion: Depende de abstracciones

✅ **Patrones de Diseño**
- Repository Pattern: Abstracción de datos
- Observer Pattern: Notificaciones
- Strategy Pattern: Cálculo de precios
- Factory Pattern: Creación de Reservas
- Dependency Injection: Inyección de dependencias

✅ **Arquitectura Limpia**
- Separación de capas
- Modelos de dominio ricos
- Repositorios abstractos
- Controladores simples

---

## 💡 Preguntas Frecuentes

**¿Qué es un diagrama UML?**
Universal Modeling Language: Notación estándar para visualizar sistemas.

**¿Por qué PlantUML?**
- Diagramas como código
- Fácil de versionar (Git)
- Genera automáticamente PNG/SVG
- Perfecto para documentación en repositorios

**¿Puedo modificar los diagramas?**
Sí, son archivos de texto. Edita directamente y regenera.

**¿Necesito especial software?**
No, puedes usar https://www.plantuml.com/plantuml/uml/ online.

---

## 📞 Información del Proyecto

| Aspecto | Detalle |
|--------|---------|
| **Nombre** | EduMentor |
| **Tipo** | Plataforma de Tutorías Virtuales |
| **Backend** | ASP.NET Core 9.0 |
| **Frontend** | HTML5, CSS3, JavaScript Vanilla |
| **Database** | MongoDB |
| **Seguridad** | JWT + BCrypt |
| **Tests** | xUnit (11 tests, 100% passing) |
| **Patrones** | Repository, Observer, Strategy, Factory |
| **Fecha** | 2 de diciembre de 2025 |

---

## 🎓 Próximos Pasos

1. **Revisar los diagramas** en PlantUML Online
2. **Leer documentación detallada** en DOCUMENTACION_UML.md
3. **Explorar el código fuente** basándote en los diagramas
4. **Ejecutar tests** para validar la arquitectura
5. **Hacer deploy** a producción

---

## 📝 Notas

- Los diagramas se regeneraron el **2 de diciembre de 2025**
- Reflejan la arquitectura **actual** del proyecto
- Incluyen todos los **patrones de diseño** implementados
- Son **documentación viva** (actualizar si hay cambios)

---

**¡Esperamos que estos diagramas te ayuden a entender EduMentor mejor!**

Para más información, ver **DOCUMENTACION_UML.md**
