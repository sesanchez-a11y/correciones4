# ⚡ QUICK START - Sistema de Roles y Creación de Cursos

## 🎯 En 5 Minutos

### 1️⃣ Iniciar Backend
```powershell
cd ".\backend"
dotnet run
```
✅ Verás: `Now listening on: http://localhost:5000`

### 2️⃣ Acceder a Frontend
```
http://localhost:5000
```

### 3️⃣ Probar Registro como Tutor
1. Click en "Iniciar sesión" (o vuelto a Inicio → Regístrate)
2. En `iniciodecesion.html`: selecciona "Registro como Tutor" (se vuelve verde)
3. Completa:
   - Email: `tutor@prueba.com`
   - Contraseña: `prueba123`
   - Nombre: `Juan`
   - Apellido: `Tutor`
   - Edad: `30`
   - Especialización: `Programación`
4. Click "Continuar"
5. ✅ Auto-login → redirige a perfil.html

### 4️⃣ Crear un Curso
1. En perfil.html, abre sidebar (≡)
2. Verás botón **"Crear Curso"** (nuevo)
3. Click en él
4. Completa formulario:
   - Nombre: `Python Avanzado`
   - Categoría: `Programación`
   - Descripción: `Aprende asyncio y type hints`
   - Precio: `29.99`
   - Nivel: `Avanzado`
   - Duración: `15` horas
   - Capacidad: `25` estudiantes
   - Temario: `Asyncio y concurrencia \n Type hints \n Decoradores`
5. Click "Enviar para Aprobación"
6. ✅ Mensaje: "✓ Curso enviado para aprobación"

### 5️⃣ Admin Aprueba Curso (Terminal)
```bash
# Login como admin
curl -X POST http://localhost:5000/api/ControladorDeSesion/login \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@edumentor.local", "password": "admin"}' \
  | jq '.token' -r > token.txt

# Ver cursos pendientes
TOKEN=$(cat token.txt)
curl -X GET http://localhost:5000/api/Cursos/admin/todos \
  -H "Authorization: Bearer $TOKEN"

# Copiar ID del curso y aprobarlo
CURSO_ID="[copiar_id_aqui]"
curl -X PUT http://localhost:5000/api/Cursos/admin/$CURSO_ID/aprobar \
  -H "Authorization: Bearer $TOKEN"
```

✅ Curso ahora está "Aprobado" y visible en catálogo público

---

## 🎨 Casos de Prueba Rápidos

### ✅ Test 1: Registro como Estudiante (sin botón Crear Curso)
```
iniciodecesion.html → "Registro como Alumno" → Registrar
→ perfil.html → Sidebar NO muestra "Crear Curso"
```

### ✅ Test 2: Admin Default
```
reseccion.html → 
  Email: admin@edumentor.local
  Password: admin
→ perfil.html (como admin)
```

### ✅ Test 3: Prevención Auto-Registro Admin
```bash
curl -X POST http://localhost:5000/api/ControladorDeSesion/register \
  -H "Content-Type: application/json" \
  -d '{"rol": "admin", "email": "hacker@mail.com", ...}'
# → 403 Forbid (rechazado)
```

---

## 🔑 Credenciales de Test

| Rol | Email | Password |
|-----|-------|----------|
| Admin | admin@edumentor.local | admin |
| Tutor | (crear) | (crear) |
| Estudiante | (crear) | (crear) |

---

## 📊 Archivos Nuevos/Modificados

### Backend
- ✅ `Modelos/Curso.cs` (NUEVO)
- ✅ `Interfaces/ICursoRepository.cs` (NUEVO)
- ✅ `Repositorios/CursoRepository.cs` (NUEVO)
- ✅ `Controllers/CursosController.cs` (NUEVO)
- ✅ `Controllers/ControladorDeSesion.cs` (modificado: BCrypt + role validation)
- ✅ `Modelos/Usuario.cs` (modificado: UserRole enum + timestamp)
- ✅ `Program.cs` (modificado: seeding + CursoRepository)

### Frontend
- ✅ `archivoshtml/perfil.html` (modificado: botón "Crear Curso")
- ✅ `archivosjs/perfil.js` (modificado: vista de crear curso + lógica de rol)

---

## 🚨 Troubleshooting

### ❌ Backend no inicia
```
Error: MongoDB connection failed
→ Verifica que MongoDB está corriendo en puerto 27017
```

### ❌ Botón "Crear Curso" no aparece
```
→ Verifica en DevTools (F12) → Console
→ localStorage.getItem('currentUser') debe tener rol: "Tutor"
```

### ❌ Error 403 al crear curso
```
→ El endpoint requiere [Authorize(Roles="Tutor")]
→ Verifica que el token tenga claim "role": "Tutor"
```

### ❌ Fronted no carga
```
→ Backend debe estar corriendo (dotnet run)
→ Accede a http://localhost:5000 (no http://localhost:3000)
```

---

## 📋 Checklist de Validación

- [ ] Backend compila sin errores
- [ ] Backend corre en puerto 5000
- [ ] Frontend carga en http://localhost:5000
- [ ] Admin default puede login
- [ ] Puedo registrar como Tutor
- [ ] Botón "Crear Curso" visible para Tutor
- [ ] Botón "Crear Curso" oculto para Estudiante
- [ ] Puedo crear un curso
- [ ] Admin puede aprobar curso
- [ ] Curso aprobado aparece en catálogo

---

## 🎓 Próximos Pasos

1. **Admin Dashboard** (futuro)
   - Panel para aprobar/rechazar cursos
   - Listar usuarios por rol
   - Analytics

2. **Inscripción de Estudiantes**
   - Botón "Inscribirse" en curso
   - Listado de estudiantes por curso
   - Progreso del estudiante

3. **Pagos** (integración Stripe)
   - Checkout
   - Historial de transacciones
   - Refunds

4. **Comunicación**
   - Email notifications
   - Mensajería entre tutor y estudiante
   - Anuncios por curso

---

## 💡 Tips

- Usa **DevTools (F12)** para inspeccionar localStorage, tokens, requests
- Usa **https://jwt.io/** para decodificar tokens y ver claims
- Usa **Postman** o **Thunder Client** para probar endpoints sin UI
- Backend logs en consola te ayudan a debug
- Si todo falla, reinicia backend (`Ctrl+C` y `dotnet run`)

---

✅ **¡Listo! Disfruta el sistema de roles y creación de cursos.**

Para documentación completa, ver:
- `IMPLEMENTACION_ROLES_Y_CURSOS.md`
- `GUIA_PRUEBAS_ROLES_Y_CURSOS.md`
- `ESTADO_FINAL_v2.0.md`
