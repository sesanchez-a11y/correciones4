# 🧪 Guía de Prueba Manual: Sistema de Roles y Creación de Cursos

## Requisitos Previos
- ✅ Backend corriendo en `http://localhost:5000`
- ✅ MongoDB corriendo localmente en `mongodb://localhost:27017`
- ✅ Frontend en `http://localhost:5000` (servido estáticamente)

---

## Prueba 1: Registro como Admin (No debería permitir)

### Pasos:
1. Accede a `http://localhost:5000/iniciodecesion.html`
2. Intenta enviar un POST a `/api/ControladorDeSesion/register` con `rol: "admin"`:

```bash
curl -X POST http://localhost:5000/api/ControladorDeSesion/register \
  -H "Content-Type: application/json" \
  -d '{
    "rol": "admin",
    "email": "hacker@ejemplo.com",
    "contrasena": "password123",
    "nombre": "Hacker",
    "apellido": "Malicioso",
    "edad": 25,
    "especializacion": "Hacking"
  }'
```

### Resultado Esperado:
```json
{
  "statusCode": 403,
  "message": "Forbid"
}
```

✅ **El servidor rechaza auto-registro como Admin**

---

## Prueba 2: Registro como Tutor (Debería funcionar)

### Pasos:
1. Accede a `http://localhost:5000/iniciodecesion.html`
2. Selecciona botón "Registro como Tutor" (se vuelve verde)
3. Completa formulario:
   - Email: `tutor1@ejemplo.com`
   - Contraseña: `tutor123`
   - Nombre: `Juan`
   - Apellido: `Pérez`
   - Edad: `30`
   - Especialización: `Programación Web`
4. Presiona "Continuar"

### Resultado Esperado:
```json
{
  "message": "Registro exitoso.",
  "user": {
    "id": "...",
    "nombre": "Juan",
    "apellido": "Pérez",
    "correo": "tutor1@ejemplo.com",
    "edad": 30,
    "especializacion": "Programación Web",
    "rol": "Tutor"
  }
}
```

✅ **Auto-login ejecutado, redirige a perfil.html**

---

## Prueba 3: Verificar botón "Crear Curso" en Perfil

### Pasos:
1. Tras registro exitoso como Tutor, estás en `perfil.html`
2. Abre el sidebar (click en hamburguesa)
3. Verifica que veas el botón "Crear Curso" (color amarillo más claro, con ícono `+`)

### Resultado Esperado:
- ✅ Botón "Crear Curso" **visible** en sidebar
- ✅ Otros botones: Historial, Mis cursos, Materiales, Horarios

---

## Prueba 4: Crear un Curso como Tutor

### Pasos:
1. En perfil.html, sidebar abierto, click en "Crear Curso"
2. Se carga formulario con campos:
   - Nombre: `React Avanzado`
   - Categoría: `Programación`
   - Descripción: `Aprende React 18 con Hooks y Context API`
   - Precio: `49.99`
   - Nivel: `Avanzado`
   - Duración: `20`
   - Capacidad: `30`
   - Temario:
     ```
     Introducción a React 18
     Hooks: useState, useEffect, useContext
     Context API vs Redux
     Performance Optimization
     ```
3. Presiona "Enviar para Aprobación"

### Resultado Esperado:
```json
{
  "message": "Curso creado exitosamente y enviado para aprobación.",
  "cursoId": "...",
  "estado": "Pendiente"
}
```

✅ **Mensaje verde: "✓ Curso enviado para aprobación exitosamente..."**

---

## Prueba 5: Registro como Estudiante (No ve botón Crear Curso)

### Pasos:
1. Accede a `http://localhost:5000/iniciodecesion.html`
2. Selecciona botón "Registro como Alumno" (se vuelve verde)
3. Completa formulario con datos de estudiante
4. Presiona "Continuar"
5. Redirige a perfil.html

### Resultado Esperado:
- ✅ Botón "Crear Curso" **NO está visible** en sidebar
- ✅ Solo ve: Historial, Mis cursos, Materiales, Horarios

---

## Prueba 6: Admin Default User

### Pasos:
1. Accede a `http://localhost:5000/reseccion.html` (login)
2. Ingresa credenciales:
   - Email: `admin@edumentor.local`
   - Password: `admin`
3. Presiona "INICIAR SESIÓN"

### Resultado Esperado:
```json
{
  "message": "Inicio de sesión exitoso.",
  "token": "eyJ...",
  "user": {
    "id": "...",
    "nombre": "Administrador",
    "apellido": "Sistema",
    "correo": "admin@edumentor.local",
    "rol": "Admin"
  }
}
```

✅ **Admin logueado, datos en localStorage**

---

## Prueba 7: Verificar JWT Claims

### Pasos:
1. Tras login exitoso, abre DevTools (F12)
2. Console → `localStorage.getItem('token')`
3. Copia el token (es un JWT)
4. Ve a https://jwt.io/
5. Pega el token en "Encoded" section
6. Verifica los claims en la sección "Payload"

### Resultado Esperado:
```json
{
  "nameid": "...",
  "email": "tutor1@ejemplo.com",
  "unique_name": "Juan",
  "role": "Tutor",
  "iat": ...,
  "exp": ...,
  "iss": "EduMentor",
  "aud": "EduMentorUsers"
}
```

✅ **Claim `role: "Tutor"` presente en el token**

---

## Prueba 8: Verificar Endpoint de Admin (Obtener todos los cursos)

### Pasos (usando terminal/Postman):
```bash
# 1. Obtener token como admin
curl -X POST http://localhost:5000/api/ControladorDeSesion/login \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@edumentor.local", "password": "admin"}' \
  | jq '.token'

# 2. Usar token en request a admin endpoint
TOKEN="eyJ..." # Pegue el token aquí

curl -X GET http://localhost:5000/api/Cursos/admin/todos \
  -H "Authorization: Bearer $TOKEN"
```

### Resultado Esperado:
```json
{
  "cursos": [
    {
      "id": "...",
      "nombre": "React Avanzado",
      "tutor": "Juan Pérez",
      "estado": "Pendiente",
      "precio": 49.99,
      "fechaCreacion": "2025-12-09T...",
      ...
    }
  ]
}
```

✅ **Admin ve todos los cursos (Pendiente, Aprobado, Rechazado)**

---

## Prueba 9: Admin Aprueba Curso

### Pasos:
```bash
TOKEN="eyJ..." # Token admin
CURSO_ID="..." # ID del curso creado en Prueba 4

curl -X PUT http://localhost:5000/api/Cursos/admin/$CURSO_ID/aprobar \
  -H "Authorization: Bearer $TOKEN"
```

### Resultado Esperado:
```json
{
  "message": "Curso aprobado exitosamente."
}
```

✅ **Curso pasa a estado "Aprobado", aparece en catálogo público**

---

## Prueba 10: Obtener Cursos Aprobados (Público)

### Pasos:
```bash
# Sin autenticación
curl -X GET http://localhost:5000/api/Cursos/aprobados
```

### Resultado Esperado:
```json
{
  "cursos": [
    {
      "id": "...",
      "nombre": "React Avanzado",
      "estado": "Aprobado",
      "tutor": "Juan Pérez",
      ...
    }
  ]
}
```

✅ **Curso aprobado es visible públicamente**

---

## Casos de Error (Esperados)

### Error 401: Token expirado/inválido
```bash
curl -X POST http://localhost:5000/api/Cursos/crear \
  -H "Authorization: Bearer INVALID_TOKEN"
```
→ **Respuesta: 401 Unauthorized**

### Error 403: No es Tutor
```bash
# Logueado como Estudiante, intenta crear curso
curl -X POST http://localhost:5000/api/Cursos/crear \
  -H "Authorization: Bearer STUDENT_TOKEN"
```
→ **Respuesta: 403 Forbidden**

### Error 403: No es Admin
```bash
# Logueado como Tutor, intenta usar endpoint admin
curl -X PUT http://localhost:5000/api/Cursos/admin/123/aprobar \
  -H "Authorization: Bearer TUTOR_TOKEN"
```
→ **Respuesta: 403 Forbidden**

---

## Checklist de Validación

- [ ] Registro como Admin rechazado (403)
- [ ] Registro como Tutor exitoso
- [ ] Botón "Crear Curso" visible para Tutor
- [ ] Botón "Crear Curso" oculto para Estudiante
- [ ] Crear curso como Tutor exitoso (estado "Pendiente")
- [ ] Admin puede obtener todos los cursos
- [ ] Admin puede aprobar curso
- [ ] Curso aprobado visible en catálogo público
- [ ] JWT contiene claim `role: "Tutor"` / `role: "Admin"`
- [ ] Endpoints sin rol adecuado retornan 403

---

## Herramientas Recomendadas

### Postman
- Importar endpoints de Cursos API
- Guardar tokens en variables
- Crear colección para pruebas

### Thunder Client (VS Code Extension)
- Lightweight, integrado en VS Code
- Ideal para pruebas rápidas

### cURL (Terminal)
- Para automatizar pruebas
- Incluida en Windows PowerShell

### DevTools del Navegador (F12)
- Console para inspeccionar localStorage
- Network tab para ver requests/responses
- Sources para debug de JavaScript

---

## Logs Esperados en Consola del Backend

```
✓ Usando MongoUsuarioRepository (DB=EduMentor) -> mongodb://localhost:27017
✓ Usando CursoRepository para gestión de cursos
✓ Usando frontend estático: C:\...\frontend
🚀 EduMentor Backend iniciando...
info: Now listening on: http://localhost:5000

[Registro tutor1@ejemplo.com como Tutor]
✓ Usuario tutor1@ejemplo.com registrado como Tutor
✓ Contraseña hasheada con BCrypt

[Login tutor1@ejemplo.com]
✓ Credenciales válidas
✓ JWT token generado con role="Tutor"

[Crear curso]
✓ Tutor Juan Pérez creó curso "React Avanzado"
✓ Curso en estado "Pendiente"
```

---

**Última Actualización**: 9 de Diciembre 2025
**Estado**: ✅ Implementación Completada
