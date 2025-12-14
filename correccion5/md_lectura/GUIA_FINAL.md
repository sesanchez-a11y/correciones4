# 🚀 GUÍA FINAL - Sistema de Autenticación EduMentor

## Cambios Realizados

### 1. **Backend** (ASP.NET Core)
- ✅ Endpoint `/login` genera JWT token válido
- ✅ Endpoint `/me` valida token y devuelve datos del usuario
- ✅ MongoDB persiste usuarios correctamente
- ✅ Password hasheado con BCrypt

### 2. **Frontend - Archivo TEST_AUTH.html**
- ✅ Interfaz completa para probar el flujo
- ✅ Registro de nuevos usuarios
- ✅ Login con email y contraseña
- ✅ Verificación de localStorage
- ✅ Prueba del endpoint /me
- ✅ Botón para ir a perfil.html

### 3. **Frontend - perfil.js**
- ✅ Espera a que el DOM esté completamente cargado (200ms)
- ✅ Intenta cargar datos desde `/me` con token JWT
- ✅ Fallback a localStorage.currentUser si `/me` falla
- ✅ Muestra datos del usuario: nombre, apellido, email, rol, teléfono
- ✅ Redirige a login si no hay sesión (después de 2 segundos)
- ✅ Logout con confirmación y limpieza de localStorage

### 4. **Frontend - iniciodecesion.js**
- ✅ Registro de usuarios
- ✅ Auto-login automático después del registro
- ✅ Guarda token JWT en localStorage
- ✅ Redirección automática a perfil.html

### 5. **Frontend - reseccion.js**
- ✅ Login con email y contraseña
- ✅ Guarda token JWT
- ✅ Guarda datos del usuario (currentUser)
- ✅ Redirección a perfil.html

---

## 📋 PASOS PARA PROBAR

### **OPCIÓN 1: Flujo Completo (RECOMENDADO)**

1. **Abre en navegador:**
   ```
   file:///c:/tareas/PROYECTO%20SOFTWARE2/carpeta/TEST_AUTH.html
   ```

2. **Haz clic en "Registrar"**
   - Los valores están pre-llenados
   - Espera ver ✓ verde

3. **Haz clic en "Revisar localStorage"**
   - Debe mostrar ✓ token existe
   - Debe mostrar ✓ usuario existe

4. **Haz clic en "Probar /me endpoint"**
   - Debe mostrar ✓ datos del usuario

5. **Haz clic en "➜ Ir a PERFIL.HTML"**
   - Deberías ver tu perfil con la información
   - **NO debe redirigir a login**

### **OPCIÓN 2: Solo Login (Si ya tienes usuario registrado)**

1. **Abre TEST_AUTH.html**

2. **En la sección "PASO 2: Iniciar sesión"**
   - Email: `testuser123@example.com`
   - Contraseña: `Test@123`
   - Haz clic en "Login"

3. **Seguir pasos 3-5 de arriba**

---

## 🧪 PRUEBAS DE DIAGNÓSTICO

Si algo falla, abre:
```
file:///c:/tareas/PROYECTO%20SOFTWARE2/carpeta/DIAGNOSTICO.html
```

Este archivo te mostrará exactamente dónde está el problema.

---

## 🔐 Sistema de Logout

**El logout funciona de dos formas:**

1. **Botón en la barra lateral** (rojo, esquina inferior)
2. **Dropdown Perfil** (arriba a la derecha) → "Cerrar sesión"

**Ambos requieren confirmación antes de cerrar sesión**

---

## ✅ Checklist Final

- [ ] Puedo registrar un nuevo usuario
- [ ] Auto-login funciona después de registro
- [ ] Veo mi perfil con mi nombre
- [ ] Veo mi email en el perfil
- [ ] No se redirige a login automáticamente
- [ ] Puedo cerrar sesión con el botón
- [ ] Al cerrar sesión, me redirige a login
- [ ] Puedo hacer login nuevamente

---

## 📝 Notas Técnicas

**localStorage:**
- `token` - JWT token para autenticación
- `currentUser` - Datos del usuario en JSON

**Rutas de archivos:**
- HTML: `/carpeta/archivoshtml/perfil.html`
- JS: `/carpeta/archivosjs/perfil.js`
- Backend: `http://localhost:5000/api/...`

**MongoDB:**
- Conexión: `mongodb://localhost:27017`
- Base de datos: `EduMentor`
- Colección: `Usuarios`

---

## 🐛 Posibles Problemas y Soluciones

| Problema | Solución |
|----------|----------|
| Perfil redirige a login | Abre DevTools (F12) → Console → Revisa los logs de error |
| No muestra datos del usuario | Verifica localStorage tiene `currentUser` |
| Token no funciona | Abre DIAGNOSTICO.html para probar /me endpoint |
| No puedo iniciar sesión | Verifica que el usuario existe en MongoDB |
| Logout no funciona | Abre Console y verifica que hay sesión activa |

---

**¿Necesitas ayuda con algo específico?**
