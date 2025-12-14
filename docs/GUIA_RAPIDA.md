# 🚀 Guía Rápida de Desarrollo - EduMentor

## ⚡ Inicio Rápido

### 1. Verificar Backend

```powershell
cd ".\backend"
dotnet run
```

**Esperado:** Backend corriendo en `http://localhost:5000`

### 2. Acceder a Frontend

- **Inicio:** `http://localhost:5000/`
- **Perfil:** `http://localhost:5000/archivoshtml/perfil.html`
- **Tienda:** `http://localhost:5000/archivoshtml/tienda.html`
- **Cursos:** `http://localhost:5000/archivoshtml/cursos.html`
- **Detalles de Curso:** `http://localhost:5000/archivoshtml/infocursos/desarrollo-web.html`

## 📝 Checklist: Agregar una Nueva Página

### Paso 1: Crear HTML
```html
<!-- archivoshtml/nueva-pagina.html -->
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width,initial-scale=1">
  <title>Nueva Página - EduMentor</title>
  
  <!-- Orden EXACTO de CSS -->
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
  <link href="../archivoscss/tienda.css" rel="stylesheet">
  <!-- <link href="../archivoscss/nueva-pagina.css" rel="stylesheet"> -->
</head>
<body>
  <div class="container">
    <h1>Nueva Página</h1>
  </div>
</body>
</html>
```

### Paso 2: Crear CSS (si es necesario)
```css
/* archivoscss/nueva-pagina.css */
/* Reutilizar variables globales */
.nuevo-elemento {
  color: var(--primary);
  background: var(--secondary);
}
```

### Paso 3: Crear JavaScript (si es necesario)
```javascript
// archivosjs/nueva-pagina.js
document.addEventListener('DOMContentLoaded', function() {
  console.log('Nueva página cargada');
});
```

### Paso 4: Agregar link en navbar
- Editar `perfil.html` o `tienda.html`
- Agregar `<a href="nueva-pagina.html">Nueva Página</a>` en el navbar

### Paso 5: Verificar
1. Hard-refresh en navegador (Ctrl+Shift+R)
2. Verificar que CSS carga (F12 → Network)
3. Verificar que no hay errores JavaScript (F12 → Console)

## 🎨 Agregar Estilos CSS

### Nunca Hacer:
```html
<!-- ❌ MAL -->
<div style="color: #1264b6; margin: 10px;">Contenido</div>
```

### Siempre Hacer:
```html
<!-- ✅ BIEN -->
<div class="contenido-principal">Contenido</div>
```

```css
/* tienda.css o tu-pagina.css */
.contenido-principal {
  color: var(--primary);
  margin: 10px;
}
```

## 🔄 Modificar Estilos Existentes

### Localizar Dónde está el CSS

```bash
# Desde PowerShell en VS Code
grep -r "nombre-de-clase" .\frontend\archivoscss\

# Ejemplo: buscar ".professor-card"
grep -r "professor-card" .\frontend\archivoscss\
```

**Resultado esperado:**
```
tienda.css:123: .professor-card {
tienda.css:125:   width: 150px;
```

### Editar CSS

1. Abrir archivo `archivoscss/tienda.css`
2. Buscar `.professor-card`
3. Modificar propiedades
4. **Hard-refresh en navegador** (Ctrl+Shift+R) para ver cambios

## 🐛 Depuración Común

### Problema: Cambios CSS no aparecen

**Solución:**
1. Hard-refresh: `Ctrl+Shift+R` (o `Cmd+Shift+R` en Mac)
2. Abrir DevTools (F12)
3. Pestaña "Network" → Verificar que CSS carga con status 200
4. Pestaña "Console" → Ver si hay errores JavaScript

### Problema: Página se ve diferente en Chrome vs Firefox

**Solución:**
1. Verificar que `<!DOCTYPE html>` está al inicio
2. Agregar `<meta charset="utf-8">` en `<head>`
3. Agregar `<meta name="viewport" content="width=device-width,initial-scale=1">`
4. Probar en ambos navegadores

### Problema: Estilos de Bootstrap no funcionan

**Solución:**
1. Verificar que Bootstrap CDN está presente: `https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css`
2. Verificar que es la primera línea de `<link>`
3. Verificar que tienes clases Bootstrap correctas: `col-md-4`, `btn btn-primary`, etc.

### Problema: localStorage no persiste

**Solución:**
1. No usar localStorage en Private/Incognito mode (no funciona)
2. Verificar en DevTools (F12) → Application → Local Storage
3. Limpiar localStorage: `localStorage.clear()` en Console

## 📊 Estructura de Datos (Ejemplo)

### Usuario en localStorage
```javascript
// Después de login
localStorage.setItem('currentUser', JSON.stringify({
  id: "507f1f77bcf86cd799439011",
  email: "usuario@example.com",
  nombre: "Juan Pérez",
  rol: "estudiante",
  fotoPerfil: "https://via.placeholder.com/100"
}));

// Recuperar
const usuario = JSON.parse(localStorage.getItem('currentUser'));
console.log(usuario.nombre); // "Juan Pérez"
```

### Curso (en infocursos/*)
```javascript
{
  titulo: "Desarrollo Web desde Cero",
  categoria: "Programación / Web",
  descripcion: "Curso intensivo...",
  temario: ["HTML", "CSS", "JavaScript"],
  docente: {
    nombre: "Carlos DeMatte",
    rating: 4.5,
    resenas: 120,
    foto: "https://via.placeholder.com/100?text=Carlos"
  }
}
```

## 🔐 Seguridad Básica

1. **Nunca guardar contraseñas** en localStorage
2. **Siempre usar HTTPS** en producción (actualmente http://localhost)
3. **Validar tokens** en cada petición API
4. **Sanitizar inputs** de usuarios antes de guardar
5. **No exponer errores internos** al usuario (mostrar mensajes genéricos)

## 📚 Referencias Útiles

- **Bootstrap 5:** https://getbootstrap.com/docs/5.3/
- **Font Awesome:** https://fontawesome.com/icons
- **MDN Web Docs:** https://developer.mozilla.org/es/
- **JavaScript Fetch API:** https://developer.mozilla.org/es/docs/Web/API/Fetch_API

## 🎯 Próximos Cambios Recomendados

1. **Tema claro/oscuro:** Agregar toggle en navbar
2. **Internacionalización (i18n):** Soporte para múltiples idiomas
3. **Progressive Web App (PWA):** Funcionar offline
4. **Notificaciones Push:** Sistema de alertas
5. **SEO:** Agregar meta tags descriptivos

---

**Última actualización:** 7 de Diciembre de 2025
**Versión:** 1.0
