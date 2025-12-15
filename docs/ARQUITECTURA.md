# 📋 Arquitectura del Frontend - EduMentor

## 🏗️ Estructura de Carpetas

```
frontend/
├── archivoshtml/          # Páginas HTML principales
│   ├── inicio.html
│   ├── iniciodecesion.html
│   ├── perfil.html
│   ├── tienda.html
│   ├── cursos.html
│   ├── miscursos.html
│   ├── reservas.html
│   ├── pagos.html
│   ├── historial.html
│   ├── confirpago.html
│   ├── reseccion.html
│   └── infocursos/        # Páginas de detalle de cursos (NEW)
│       ├── desarrollo-web.html
│       ├── excel-principiantes.html
│       ├── fotografia-smartphone.html
│       ├── marketing-digital.html
│       └── edicion-video-premiere.html
├── archivoscss/           # Estilos CSS
│   ├── tienda.css         # Estilos globales + tienda
│   ├── perfil.css         # Estilos específicos de perfil
│   ├── cursos.css
│   ├── inicio.css
│   ├── miscursos.css
│   └── ... (otros CSS)
├── archivosjs/            # Scripts JavaScript
│   ├── reseccion.js       # Login/Register
│   ├── perfil.js          # Cargar datos de perfil
│   ├── tienda.js
│   ├── cursos.js
│   └── ... (otros JS)
├── img/                   # Imágenes
├── diagnostico/           # Páginas de debug (Testing)
└── ARQUITECTURA.md        # Este archivo

```

## 🎨 Estrategia de CSS

### Jerarquía de Carga

Cada página HTML debe cargar CSS en este orden **exacto**:

```html
<!-- 1. Bootstrap (framework base) -->
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">

<!-- 2. Estilos globales -->
<link href="../archivoscss/tienda.css" rel="stylesheet">

<!-- 3. Estilos específicos de la página (si aplica) -->
<link href="../archivoscss/perfil.css" rel="stylesheet">
<link href="../archivoscss/cursos.css" rel="stylesheet">
```

### `tienda.css` (Global)

**Responsabilidad:** Variables CSS, reglas globales, componentes compartidos.

```css
:root {
  --primary: #1264b6;      /* Azul corporativo */
  --warning: #F5BE6B;      /* Amarillo navbar */
  --secondary: #f8f9fa;
  --light: #e9ecef;
  --dark: #212529;
}
```

**Componentes incluidos:**
- `.top-bar` — Barra negra superior
- `.navbar` — Navegación amarilla + responsive
- `.hamburguesa`, `.hamburger-icon` — Menú mobile
- `.utility-links` — Buscar, notificaciones, tienda, perfil
- `.badges`, `.price-tag` — Etiquetas de precio
- `.professor-grid`, `.professor-card` — Grid de profesores
- `.filter-section` — Filtros de búsqueda
- `.ratings` — Estrellas de valoración

**Regla de oro:** Sin `style="..."` inline en HTML; todo en CSS.

### `perfil.css` (Específico)

**Responsabilidad:** Estilos exclusivos de la página de perfil.

**Componentes incluidos:**
- `.avatar` — Foto de usuario circular
- `.offcanvas` — Sidebar lateral
- `.list-group-item.active-menu-link` — Menú activo
- `.main-content-area` — Área principal de contenido
- `.table thead` — Tablas de historial/reservas
- Media queries — Responsive para mobile/tablet

**Nota:** Reutiliza variables CSS globales de `tienda.css`.

### Reglas de Mantenimiento CSS

1. **Nunca duplicar variables:** Usar `:root {}` global desde `tienda.css`.
2. **No usar `!important`:** Si necesitas sobrescribir, verifica el orden de carga.
3. **Nombres de clase semánticos:** `.btn-primary-custom` en lugar de `.blue-button`.
4. **Media queries al final de cada CSS:** Agrupa todo responsive al final del archivo.
5. **Colores via variables:** Nunca hardcodear `#1264b6` en CSS; usar `var(--primary)`.

## 🔌 Backend - Endpoints Esperados

El backend (`backend`) expone estos endpoints usados por el frontend:

| Método | Endpoint | Propósito | Headers Requeridos |
|--------|----------|-----------|-------------------|
| `POST` | `/api/ControladorDeSesion/register` | Crear cuenta | `Content-Type: application/json` |
| `POST` | `/api/ControladorDeSesion/login` | Iniciar sesión | `Content-Type: application/json` |
| `GET` | `/api/ControladorDeSesion/me` | Obtener datos de perfil | `Authorization: Bearer <token>` |

**Formato de respuesta esperada:**
```json
{
  "success": true,
  "message": "Login exitoso",
  "token": "eyJhbGc...",
  "currentUser": {
    "id": "...",
    "email": "user@example.com",
    "nombre": "Juan",
    "rol": "estudiante"
  }
}
```

## 📱 Flujo de Autenticación (Frontend)

### 1. Reseccion (Login/Register) - `reseccion.js`

```javascript
// POST /api/ControladorDeSesion/register
const response = await fetch('/api/ControladorDeSesion/register', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ email, password, nombre })
});

// Guardar token en localStorage
localStorage.setItem('token', data.token);
localStorage.setItem('currentUser', JSON.stringify(data.currentUser));

// Redirigir a perfil
window.location.href = '/archivoshtml/perfil.html';
```

### 2. Perfil - `perfil.js`

```javascript
// GET /api/ControladorDeSesion/me
const token = localStorage.getItem('token');
const response = await fetch('/api/ControladorDeSesion/me', {
  headers: { 'Authorization': `Bearer ${token}` }
});

// Renderizar datos en DOM
document.getElementById('user-name').textContent = currentUser.nombre;
```

### 3. Storage Local

**Variables guardadas en `localStorage`:**
- `token` — JWT para autenticación
- `currentUser` — Objeto JSON con datos del usuario

## 🛠️ Cambios Recientes (Diciembre 2025)

### Phase 6: Curso Cards + Docentes

**Cambios en `cursos.html` (líneas 190-274):**
- Reemplazó lista simple por grid Bootstrap de 5 tarjetas
- Cada tarjeta es un `<a>` link a `infocursos/*.html`

**Nuevos archivos en `infocursos/`:**
1. `desarrollo-web.html` — Carlos DeMatte (4.5⭐)
2. `excel-principiantes.html` — Laura DataCoach (5.0⭐)
3. `fotografia-smartphone.html` — Miguel Lens (4.7⭐)
4. `marketing-digital.html` — Ana Growth (4.0⭐)
5. `edicion-video-premiere.html` — Javier FilmPro (4.6⭐)

**Cada página incluye:**
- Descripción del curso
- Temario (lista de temas)
- Tarjeta de docente con foto + rating
- Botón "Volver a Cursos"

## ⚠️ Problemas Históricos Resueltos

| Problema | Solución | Estado |
|----------|----------|--------|
| Perfil desaparecía tras login | JS defensivos + localStorage retry | ✅ Resuelto |
| Tienda layout roto | Cerrar navbar correctamente + mover filtros fuera | ✅ Resuelto |
| CSS duplicado/truncado | Consolidar en tienda.css + perfil.css | ✅ Resuelto |
| Offcanvas desaparecía | Verificar orden de carga CSS + cerrar etiquetas | ✅ Resuelto |
| 47 matches inline styles | Mover la mayoría a CSS externo | ⚠️ Parcial |

## 🔍 Inline Styles Pendientes de Limpiar

**Archivo** | **Línea aprox.** | **Elemento** | **Acción**
-----------|-----------------|-------------|----------
`perfil.html` | 67 | offcanvas | `style="width: 280px; border-radius: 0 15px 15px 0;"` — Bajar prioridad (structural)
`miscursos.html` | varios | Múltiples | Extraer a CSS
`pagos.html` | varios | Múltiples | Extraer a CSS
`diagnostico/*` | varios | Testing pages | Extraer a CSS (low priority)
`perfil.js` | plantillas | HTML dinámico | Generar clases en lugar de estilos inline

**Opción 1 (Recomendada):** Mover todo a `css` externo, crear `inline-cleanup.css`.
**Opción 2:** Refactorizar `perfil.js` para generar HTML sin `style="..."`.
**Opción 3:** Dejar como está (baja prioridad, funciona correctamente).

## 🚀 Próximos Pasos (Sugeridos)

1. **Integración con Base de Datos:** Conectar MongoDB a curso detail pages para datos dinámicos.
2. **Imagenes Reales:** Reemplazar `via.placeholder.com` con URLs de imágenes reales.
3. **Emails Reales:** Actualizar `docente@edumentor.example` con contactos verdaderos.
4. **Ratings Dinámicos:** Traer scores desde API backend.
5. **Carrito de Compras:** Implementar funcionalidad de `tienda.html`.
6. **Dashboard Admin:** Panel para gestionar cursos/docentes.

## 📞 Contacto & Soporte

- **Backend:** ASP.NET Core (.NET 9) en `backend/`
- **Base de Datos:** MongoDB `localhost:27017`
- **Frontend Port:** `http://localhost:5000`
- **Servidor:** Kestrel

---

**Última actualización:** 7 de Diciembre de 2025
