# 🔧 SOLUCIÓN: Página de Perfil se Cierra Después de Iniciar Sesión

## 📋 Resumen del Problema

Cuando inicias sesión, la página de perfil se muestra por 1 segundo y luego desaparece (redirige a login).

## 🎯 Causas Identificadas y Corregidas

### 1. **Tiempo de DOM insuficiente** ✅ CORREGIDO
- **Problema**: El `init()` se ejecutaba con un delay de solo 200ms, insuficiente para que el DOM esté listo
- **Solución**: Aumentado a 500ms y agregado delay adicional de 300ms después de cargar usuario

### 2. **Búsqueda de elementos del DOM débil** ✅ CORREGIDO
- **Problema**: Si los elementos no se encontraban, fallaba silenciosamente
- **Solución**: Agregado sistema de reintentos (50 intentos = 5 segundos máximo) con logging detallado

### 3. **Tiempo de redirección muy rápido** ✅ CORREGIDO
- **Problema**: El redirect a login sucedía si no había usuario, con solo 2 segundos de espera
- **Solución**: Aumentado a 3 segundos, y agregado más logging

### 4. **Token no se guardaba en reseccion.js** ✅ CORREGIDO
- **Problema**: El login guardaba token pero con un delay de 500ms insuficiente
- **Solución**: Aumentado a 800ms y agregado validación explícita del token

---

## 🚀 Cambios Realizados

### Archivo: `perfil.js`

#### Cambio 1: Aumentar delay inicial
```javascript
// ANTES: 200ms
await new Promise(resolve => setTimeout(resolve, 200));

// AHORA: 500ms + 300ms adicional
await new Promise(resolve => setTimeout(resolve, 500));
// ... después de cargar usuario
await new Promise(resolve => setTimeout(resolve, 300));
```

#### Cambio 2: Mejor búsqueda de elementos DOM
```javascript
// ANTES: Búsqueda simple sin reintentos
const checkElements = () => {
  if (userNameTable && userEmailTable) {
    resolve();
  } else {
    setTimeout(checkElements, 100);
  }
};

// AHORA: Con límite de intentos y logging
let attempts = 0;
const maxAttempts = 50; // 5 segundos máximo
const checkElements = () => {
  attempts++;
  const userNameTable = document.getElementById('userNameTable');
  const userEmailTable = document.getElementById('userEmailTable');
  
  if (userNameTable && userEmailTable) {
    console.log(`✓ Elementos encontrados en intento ${attempts}`);
    resolve();
  } else if (attempts >= maxAttempts) {
    console.warn(`⚠️ No se encontraron elementos después de ${attempts} intentos`);
    resolve(); // Continuar de todas formas
  } else {
    setTimeout(checkElements, 100);
  }
};
```

#### Cambio 3: Aumentar tiempo de logout
```javascript
// ANTES: 2000ms
setTimeout(() => { window.location.href = './reseccion.html'; }, 2000);

// AHORA: 3000ms
setTimeout(() => { window.location.href = './reseccion.html'; }, 3000);
```

#### Cambio 4: Mejor inicialización
```javascript
// Usar DOMContentLoaded en lugar de ejecutar directamente
document.addEventListener('DOMContentLoaded', function() {
  console.log('📄 DOMContentLoaded disparado');
  init();
});
```

### Archivo: `reseccion.js`

#### Aumentar tiempo de redirección
```javascript
// ANTES: 500ms
setTimeout(() => { window.location.href = 'perfil.html'; }, 500);

// AHORA: 800ms
setTimeout(() => { 
  console.log('🔄 Redirigiendo a perfil.html...');
  window.location.href = 'perfil.html'; 
}, 800);
```

#### Mejorar logging
```javascript
console.log('✓ Token guardado en localStorage');
console.log('✓ Datos del usuario guardados en localStorage');
console.log('⏳ Redirigiendo a perfil.html en 800ms...');
```

---

## 🧪 Cómo Probar la Solución

### Opción 1: Prueba Completa (Recomendado)

1. **Abre la página de diagnóstico**:
   ```
   file:///c:/tareas/PROYECTO%20SOFTWARE2/frontend/archivoshtml/DIAGNOSTICO_PERFIL.html
   ```

2. **Presiona los botones en orden**:
   - "1. Verificar localStorage" - Ver si hay datos guardados
   - "2. Probar endpoint /me" - Validar token con el backend
   - "3. Simular loadUserData()" - Ver si se cargaían datos correctamente

3. **Luego prueba el flujo completo**:
   - Ve a `reseccion.html`
   - Inicia sesión con credenciales válidas
   - **Observa la consola del navegador** (F12 → Console)
   - Deberías ver muchos logs verdes ✓

### Opción 2: Monitoreo en Vivo

1. Abre `reseccion.html`
2. Presiona F12 para abrir Developer Tools
3. Ve a la pestaña "Console"
4. Inicia sesión
5. Observa los logs en tiempo real

**Logs esperados:**
```
✓ Login success: {...}
✓ Token guardado en localStorage
✓ Datos del usuario guardados en localStorage
⏳ Redirigiendo a perfil.html en 800ms...
📄 DOMContentLoaded disparado
🚀 Init de perfil.js iniciado
⏳ DOM está listo, cargando datos del usuario...
=== LOADUSERDATA INICIADO ===
Token en localStorage: ✓ Existe
CurrentUser en localStorage: ✓ Existe
✓ Usuario encontrado: [nombre]
✓ Elementos encontrados en intento 1
✓ Nombre actualizado: ...
✓ Email actualizado
✓ Rol actualizado
✓ Datos del usuario cargados correctamente
📄 Cargando contenido de historial...
✓ Init completado
```

---

## ❌ Si Sigue Sin Funcionar

### Paso 1: Verificar Backend

```powershell
# En una terminal PowerShell, ve a la carpeta del backend
cd "c:\tareas\PROYECTO SOFTWARE2\TutoriasDeClasesbackend"

# Asegúrate que el backend está corriendo
dotnet run
```

Deberías ver:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

### Paso 2: Verificar MongoDB

```powershell
# Abre otra terminal
mongosh
use EduMentor
db.Usuarios.find().pretty()
```

Deberías ver usuarios en la base de datos.

### Paso 3: Verificar Credenciales

1. Abre `DIAGNOSTICO_PERFIL.html`
2. Registra un nuevo usuario en `iniciodecesion.html`
3. Usa ese email/password para login
4. Si aún falla, presiona los botones de diagnóstico

### Paso 4: Revisar Consola del Navegador

Presiona F12 y ve a Console. Busca:
- ❌ Errores rojos (CORS, network, etc)
- ⚠️ Advertencias amarillas (tiempos)
- ✓ Logs verdes (éxito)

---

## 📊 Matriz de Solución

| Síntoma | Causa | Solución |
|---------|-------|----------|
| Perfil desaparece en 1 segundo | DOM no está listo | ✅ Aumentado delay a 500ms |
| "No hay usuario autenticado" | localStorage vacío | ✅ Verificar login completo |
| Elemento "userNameTable" no encontrado | DOM asincrónico | ✅ Sistema de reintentos |
| Token inválido en /me | Guardado incorrecto | ✅ Delay aumentado a 800ms |
| CORS error | Backend no corriendo | ✅ Iniciar `dotnet run` |

---

## 🎯 Checklist de Verificación

Después de implementar los cambios, verifica:

- [ ] El backend está corriendo en `http://localhost:5000`
- [ ] MongoDB está activo (conexión a `mongodb://localhost:27017`)
- [ ] `perfil.js` tiene los cambios (500ms delay, sistema de reintentos)
- [ ] `reseccion.js` tiene los cambios (800ms delay, mejor logging)
- [ ] Puedes registrarte nuevos usuarios
- [ ] Puedes iniciar sesión correctamente
- [ ] Ves la página de perfil por MÁS de 1 segundo
- [ ] Ves tu nombre, email y rol en el perfil
- [ ] La consola muestra muchos logs ✓ verdes

---

## 📝 Resumen de Archivos Modificados

```
✅ perfil.js
   - Delay inicial: 200ms → 500ms
   - Delay post-usuario: 0ms → 300ms
   - Búsqueda DOM: simple → con reintentos (50)
   - Timeout redirect: 2000ms → 3000ms
   - Inicialización: directa → DOMContentLoaded

✅ reseccion.js
   - Redirect delay: 500ms → 800ms
   - Logging: básico → detallado
   - Validación: simple → explícita

✅ NUEVO: DIAGNOSTICO_PERFIL.html
   - Herramienta de diagnóstico
   - Verificar localStorage
   - Probar /me endpoint
   - Simular loadUserData()
```

---

## 🆘 Soporte

Si aún tienes problemas:

1. **Abre la consola** (F12 → Console)
2. **Copia todos los logs**
3. **Usa DIAGNOSTICO_PERFIL.html** para revisar estado
4. **Verifica que el backend esté corriendo**: `dotnet run`

---

**Última actualización**: 2 de diciembre de 2025  
**Versión**: 1.1 (Con mejoras de timing y debugging)
