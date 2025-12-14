# 📋 Diagrama de Flujo: Login y Validación → Perfil

## 🎯 Archivos Involucrados

### **FRONTEND (Cliente)**

#### 1. **reseccion.html** ← Página de Login
- **Ubicación**: `c:\tareas\PROYECTO SOFTWARE2\frontend\archivoshtml\reseccion.html`
- **Función**: Formulario HTML para ingreso de credenciales
- **IDs clave**:
  - `loginForm` - Formulario
  - `login-email` - Campo de correo
  - `login-password` - Campo de contraseña
  - `loginMessage` - Div para mensajes (éxito/error)
- **Clase CSS**: `reseccion-page` (identifica la página)

#### 2. **reseccion.js** ← Script que valida el login
- **Ubicación**: `c:\tareas\PROYECTO SOFTWARE2\frontend\archivosjs\reseccion.js`
- **Función principal**: Manejar evento `submit` del formulario
- **Código clave**:
  ```javascript
  loginForm.addEventListener('submit', async function (e) {
    e.preventDefault();
    const email = document.getElementById('login-email').value.trim();
    const password = document.getElementById('login-password').value;
    
    // HACER POST al backend
    const resp = await fetch('http://localhost:5000/api/ControladorDeSesion/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ Email: email, Password: password })
    });
    
    const data = await resp.json();
    if (resp.ok) {
      // Guardar token y usuario en localStorage
      localStorage.setItem('token', data.token);
      localStorage.setItem('currentUser', JSON.stringify(data.user));
      
      // Redirigir a perfil.html después de 800ms
      setTimeout(() => {
        window.location.href = 'perfil.html';
      }, 800);
    }
  });
  ```
- **Qué hace**:
  1. Detecta si se está en página `reseccion-page`
  2. Espera evento `submit` del formulario
  3. Lee email y contraseña de inputs
  4. Hace POST a `/api/ControladorDeSesion/login`
  5. Si éxito: Guarda token + user en localStorage
  6. Redirige a `perfil.html` después de 800ms

---

### **BACKEND (Servidor ASP.NET Core)**

#### 3. **controladordesecion.cs** ← Valida credenciales
- **Ubicación**: `c:\tareas\PROYECTO SOFTWARE2\TutoriasDeClasesbackend\Controllers\controladordesecion.cs`
- **Endpoint**: `POST /api/ControladorDeSesion/login`
- **Función**:
  ```csharp
  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginModel model)
  {
    var user = await _usuarioRepo.ValidateCredentialsAsync(model.Email, pwd);
    if (user == null) return Unauthorized("Credenciales inválidas");
    
    // Generar JWT token
    var token = new JwtSecurityToken(...);
    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    
    // Devolver token + datos públicos del usuario
    return Ok(new { 
      message = "Inicio de sesión exitoso.",
      token = tokenString,
      user = { id, nombre, apellido, correo, rol, ... }
    });
  }
  ```
- **Qué hace**:
  1. Recibe Email y Password
  2. Busca usuario en MongoDB
  3. Valida contraseña
  4. Si válido: Genera JWT token
  5. Devuelve `{ token, user }` como JSON

---

### **FRONTEND (Después del login)**

#### 4. **perfil.html** ← Página destino después del login
- **Ubicación**: `c:\tareas\PROYECTO SOFTWARE2\frontend\archivoshtml\perfil.html`
- **Función**: Mostrar datos del usuario autenticado
- **IDs clave para actualizar datos**:
  - `userNameTable` - Mostrar nombre
  - `userEmailTable` - Mostrar correo
  - `userRolTable` - Mostrar rol
  - Otros campos según datos de usuario
- **Clase CSS**: `perfil-page`

#### 5. **perfil.js** ← Script que carga y muestra datos del perfil
- **Ubicación**: `c:\tareas\PROYECTO SOFTWARE2\frontend\archivosjs\perfil.js`
- **Función principal**: Cargar datos del usuario autenticado
- **Código clave**:
  ```javascript
  document.addEventListener('DOMContentLoaded', function() {
    console.log('📄 DOMContentLoaded disparado');
    init();
  });
  
  async function init() {
    // Esperar 500ms para que DOM esté listo
    await new Promise(resolve => setTimeout(resolve, 500));
    
    // Cargar datos del usuario
    await loadUserData();
  }
  
  async function loadUserData() {
    // Obtener token de localStorage
    const token = localStorage.getItem('token');
    
    // Hacer GET a /api/ControladorDeSesion/me con Bearer token
    const response = await fetch('http://localhost:5000/api/ControladorDeSesion/me', {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    
    const user = await response.json();
    
    // Actualizar HTML con datos
    document.getElementById('userNameTable').textContent = user.nombre;
    document.getElementById('userEmailTable').textContent = user.correo;
    document.getElementById('userRolTable').textContent = user.rol;
  }
  ```
- **Qué hace**:
  1. Espera que DOMContentLoaded se dispare
  2. Espera 500ms adicionales
  3. Lee el token de localStorage
  4. Hace GET a `/api/ControladorDeSesion/me` con token
  5. Actualiza los elementos HTML con datos del usuario

---

## 🔄 Flujo Completo: Login → Perfil

```
┌─────────────────────────────────────────────────────────────────┐
│ 1. Usuario abre reseccion.html (Página de Login)               │
│    ✓ HTML carga reseccion.js                                    │
│    ✓ Clase CSS "reseccion-page" detectada                       │
└─────────────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────────┐
│ 2. Usuario ingresa Email y Contraseña y hace SUBMIT             │
│    ✓ reseccion.js detecta evento submit                         │
│    ✓ Lee valores de #login-email y #login-password              │
└─────────────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────────┐
│ 3. reseccion.js hace POST a:                                    │
│    http://localhost:5000/api/ControladorDeSesion/login         │
│                                                                  │
│    Body: { Email: "...", Password: "..." }                      │
└─────────────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────────┐
│ 4. BACKEND: controladordesecion.cs recibe POST                  │
│    ✓ Valida Email y contraseña en MongoDB                       │
│    ✓ Genera JWT token                                           │
│    ✓ Devuelve: { token: "...", user: {...} }                    │
└─────────────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────────┐
│ 5. reseccion.js recibe respuesta exitosa                        │
│    ✓ localStorage.setItem('token', data.token)                  │
│    ✓ localStorage.setItem('currentUser', JSON.stringify(...))   │
│    ✓ Espera 800ms                                               │
│    ✓ window.location.href = 'perfil.html'                       │
└─────────────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────────┐
│ 6. Browser navega a perfil.html                                 │
│    ✓ HTML carga perfil.js                                       │
│    ✓ DOMContentLoaded se dispara                                │
│    ✓ Llama a init()                                             │
└─────────────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────────┐
│ 7. perfil.js espera 500ms + 300ms adicionales                   │
│    ✓ Asegura que DOM está completamente listo                   │
│    ✓ Llama a loadUserData()                                     │
└─────────────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────────┐
│ 8. loadUserData() hace GET a:                                   │
│    http://localhost:5000/api/ControladorDeSesion/me             │
│                                                                  │
│    Header: Authorization: Bearer eyJhbGc...                      │
└─────────────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────────┐
│ 9. BACKEND: controladordesecion.cs recibe GET /me               │
│    ✓ Verifica que Authorization header es válido                │
│    ✓ Decodifica JWT token                                       │
│    ✓ Busca usuario en MongoDB                                   │
│    ✓ Devuelve: { id, nombre, apellido, correo, rol, ... }      │
└─────────────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────────┐
│ 10. perfil.js recibe datos del usuario                          │
│     ✓ document.getElementById('userNameTable').textContent =    │
│        user.nombre                                              │
│     ✓ document.getElementById('userEmailTable').textContent =   │
│        user.correo                                              │
│     ✓ document.getElementById('userRolTable').textContent =     │
│        user.rol                                                 │
│     ✓ ... actualiza más campos según estructura                 │
└─────────────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────────┐
│ 11. ✓ PERFIL VISIBLE EN PANTALLA CON DATOS DEL USUARIO          │
│                                                                  │
│     Nombre: Juan Perez                                          │
│     Correo: usuario@test.com                                    │
│     Rol: Alumno                                                 │
│     ... otros datos                                             │
└─────────────────────────────────────────────────────────────────┘
```

---

## ⚠️ Puntos Críticos (Dónde pueden fallar las cosas)

| # | Punto | Problema | Solución |
|---|-------|----------|----------|
| 1 | `reseccion.html` | Formulario sin IDs correctos | Verificar `id="loginForm"`, `id="login-email"`, `id="login-password"` |
| 2 | `reseccion.js` | No carga o no está vinculado | Verificar `<script src="../archivosjs/reseccion.js"></script>` |
| 3 | POST a backend | Error de CORS | Backend debe tener `AddCors("AllowAll")` |
| 4 | POST a backend | Credenciales rechazadas | Usuario no existe o contraseña es incorrecta |
| 5 | POST a backend | No devuelve token | Backend debe devolver `{ token, user }` |
| 6 | localStorage | Token no se guarda | Verificar que localStorage está disponible |
| 7 | Redirect a perfil | Ocurre demasiado rápido | Token no se guardó aún (solución: 800ms delay) |
| 8 | perfil.html | Carga pero sin datos | perfil.js no cargó o no se ejecutó |
| 9 | perfil.js | No obtiene token | localStorage.getItem('token') retorna null |
| 10 | GET /me | Falla autenticación | Token expirado o inválido, falta Bearer |
| 11 | GET /me | No encuentra usuario | Email en token no coincide con DB |
| 12 | perfil.html | Elementos no encontrados | IDs como `userNameTable` no existen en HTML |

---

## 🔍 Cómo Diagnosticar Problemas

### **Paso 1: Abre consola F12 mientras haces login**
```
Presiona: F12 → Console
Intenta login
Busca mensajes como:
  ✓ Login success:
  ✓ Token guardado en localStorage
  ✓ Redirigiendo a perfil.html
```

### **Paso 2: Revisa localStorage**
```
F12 → Application → Local Storage → http://localhost:5000
Busca:
  - token: eyJhbGc... (debe existir)
  - currentUser: {"id": "...", "nombre": "...", ...}
```

### **Paso 3: Revisa Network tab**
```
F12 → Network
Mientras haces login, busca:
  - POST /api/ControladorDeSesion/login
    Response debe tener: { token, user }
    Status debe ser: 200
  
  - GET /api/ControladorDeSesion/me
    Response debe tener datos del usuario
    Status debe ser: 200
    Header debe tener: Authorization: Bearer ...
```

### **Paso 4: Verifica que los IDs existen en perfil.html**
```javascript
// En consola de perfil.html:
document.getElementById('userNameTable')
document.getElementById('userEmailTable')
document.getElementById('userRolTable')
// Si retorna null, el elemento no existe
```

---

## 📦 Resumen de Archivos

| Archivo | Tipo | Ubicación | Función |
|---------|------|-----------|---------|
| **reseccion.html** | HTML | `frontend/archivoshtml/` | Formulario login |
| **reseccion.js** | JS | `frontend/archivosjs/` | Maneja login, valida, redirige |
| **controladordesecion.cs** | C# | `TutoriasDeClasesbackend/Controllers/` | Endpoint /login, genera token |
| **perfil.html** | HTML | `frontend/archivoshtml/` | Página destino, muestra datos |
| **perfil.js** | JS | `frontend/archivosjs/` | Carga datos, actualiza HTML |

---

## ✅ Checklist: Qué debe suceder para que funcione

- [ ] Backend corriendo en `http://localhost:5000`
- [ ] Endpoint `/api/ControladorDeSesion/login` responde
- [ ] Endpoint `/api/ControladorDeSesion/me` responde con JWT
- [ ] CORS habilitado en backend
- [ ] `reseccion.html` carga `reseccion.js`
- [ ] `reseccion.js` vinculado en HTML con `<script src>`
- [ ] Usuario existe en MongoDB
- [ ] Contraseña es correcta
- [ ] `perfil.html` carga `perfil.js`
- [ ] `perfil.js` busca IDs correctos: `userNameTable`, `userEmailTable`, `userRolTable`
- [ ] localStorage funciona
- [ ] Redirect ocurre con 800ms de delay

