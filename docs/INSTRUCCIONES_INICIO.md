INSTRUCCIONES PARA INICIAR EL PROYECTO (Windows / PowerShell)

⚡ **OPCIÓN RÁPIDA** (Recomendado): Ejecuta UN SOLO COMANDO
================================================================================
```powershell
powershell -ExecutionPolicy Bypass -File "C:\tareas\PROYECTO SOFTWARE2\start-all.ps1"
```
Esto inicia: MongoDB + Backend (.NET) + Servidor Estático (Python) automáticamente.
Luego abre en tu navegador: http://localhost:5500/iniciosesion.html

Si prefieres hacerlo paso a paso, sigue las instrucciones manuales abajo.
================================================================================

Este documento explica, paso a paso, cómo preparar y arrancar el proyecto para que los usuarios puedan registrarse. Incluye requisitos, cómo iniciar la base de datos (MongoDB), cómo ejecutar el backend (ASP.NET Core) y cómo abrir el frontend estático.

REQUISITOS
- Windows 10/11
- PowerShell (v5.1 está bien)
- .NET SDK 7 o 8/9 instalado (se usa .NET 9 en el proyecto). Comprueba con: `dotnet --version`
- MongoDB (local) o una instancia MongoDB accesible
- Navegador (Chrome/Edge/Firefox)
- (Opcional) Live Server extension o un servidor HTTP estático para servir las páginas front-end

1) Verificar .NET y PowerShell
Abra PowerShell y ejecute:

```powershell
dotnet --version
pwsh -c "echo 'PowerShell disponible'"
```

Debe mostrar la versión de .NET (ej. `9.0.x`) y no devolver errores.

2) Preparar MongoDB (local)
El backend utiliza MongoDB (por defecto se espera `mongodb://localhost:27017`). Si no tiene MongoDB instalado localmente, instale y arranque MongoDB o use una instancia en la nube (Atlas) y copie la cadena de conexión.

- Instalar MongoDB en Windows: use el instalador oficial (https://www.mongodb.com/try/download/community) y luego arranque el servicio:

```powershell
# Si instalaste MongoDB como servicio, normalmente ya está en ejecución.
# Para arrancar el servicio manualmente:
net start MongoDB
# Para detenerlo:
net stop MongoDB
```

- Verificar que MongoDB escucha:

```powershell
# Usando administrador de puertos (opcional)
netstat -an | Select-String ":27017"
```

3) Revisar configuración del backend
- El backend está en `TutoriasDeClasesbackend/`.
- Abre `appsettings.json` (o `Program.cs`) para confirmar la cadena de conexión a MongoDB y el puerto en el que corre la API (por defecto suele ser `http://localhost:5000` o `http://localhost:5001` si HTTPS). Si necesitas cambiar la conexión, edita `appsettings.json` o las variables de entorno.

4) (Opcional) Habilitar creación del admin por defecto
- El proyecto suele incluir un bloque de "seeding" que crea un admin por defecto (correo `admin@edumentor.local`, password `admin`) en entornos de desarrollo o si está activada la opción `EnableDefaultAdmin`.
- Para asegurarte que el seed se ejecuta, arranca la app en entorno `Development` o añade la clave `EnableDefaultAdmin` en `appsettings.Development.json` o como variable de entorno:

```powershell
# Desde PowerShell, para ejecutar el backend en modo Development:
$env:ASPNETCORE_ENVIRONMENT = 'Development'
# Si quieres añadir variable para habilitar explícitamente el seed:
$env:EnableDefaultAdmin = 'true'
```

5) Ejecutar backend (API)
1. Abre PowerShell en la carpeta del backend:

```powershell
cd "c:\tareas\PROYECTO SOFTWARE2\TutoriasDeClasesbackend"
```

2. Restaurar paquetes y compilar:

```powershell
dotnet restore
dotnet build
```

3. Ejecutar la aplicación:

```powershell
dotnet run
```

- Observa la salida en consola. Debe indicar algo como "Now listening on: http://localhost:5000".
- Si el seed está activo, verás mensajes en la consola sobre la creación del usuario admin.

6) Probar endpoints (opcional)
- Para comprobar que el backend funciona, prueba el endpoint de login o `GET /api/ControladorDeSesion/me` usando `curl` o Postman.

Ejemplo usando curl para login (reemplaza URL si es distinto):

```powershell
curl -X POST "http://localhost:5000/api/ControladorDeSesion/login" -H "Content-Type: application/json" -d '{"Email":"admin@edumentor.local","Password":"admin"}'
```

La respuesta debe incluir un `token` (JWT) y datos del `user`.

7) Iniciar el frontend (páginas estáticas)
Tienes dos formas de abrir el frontend:

A) Abrir archivos directamente en el navegador
- Ve a `c:\tareas\PROYECTO SOFTWARE2\frontend\archivoshtml\reservas.html` y haz doble clic para abrirlo en tu navegador. Algunas funciones que requieren CORS o fetch desde el backend pueden no funcionar si lo abres como archivo local.

B) Servir las páginas con un servidor estático (recomendado)
- Instalar Python 3 y ejecutar un servidor simple desde la carpeta `frontend/archivoshtml`:

```powershell
cd "c:\tareas\PROYECTO SOFTWARE2\frontend\archivoshtml"
# Python 3 (PowerShell)
py -3 -m http.server 5500
# o si 'python' está en PATH:
python -m http.server 5500
```

- Abre `http://localhost:5500/reservas.html` en el navegador.

Alternativa: usar la extensión Live Server de VS Code o cualquier servidor estático (http-server de npm, etc.).

8) Registrar usuarios (frontend)
- Abre la página de registro `iniciodecesion.html` (o la UI de login/registro que hayas integrado) desde el servidor estático o navegador.
- Completa el formulario y selecciona el rol (`Alumno` o `Tutor`).
- El frontend envía la petición al backend (`/api/ControladorDeSesion/register`) — asegúrate de que la URL del fetch coincide con la URL del backend (por ejemplo, `http://localhost:5000/api/ControladorDeSesion/register`).

9) Comprobaciones si algo falla
- Revisa la consola del navegador (F12 → Console) para errores JS o CORS.
- Revisa la salida de la consola donde ejecutaste `dotnet run` para ver errores del backend.
- Verifica la cadena de conexión a MongoDB en `appsettings.json` y que MongoDB esté en ejecución.
- Si ves `Credenciales inválidas` al intentar loguear, puede ser por el hash de contraseña; asegúrate de que el registro creó la entrada correctamente en MongoDB.

10) Credenciales de administrador (para pruebas)
- Si el seed se ejecutó, usa:
  - Email: `admin@edumentor.local`
  - Password: `admin`
- Conéctate a `http://localhost:5000/api/ControladorDeSesion/login` (o usa el formulario UI) para generar el JWT de administrador.

11) Apagar servicios
- Para detener el backend: en la consola donde se ejecuta `dotnet run`, presiona `Ctrl+C`.
- Para detener MongoDB (si se ejecuta como servicio):

```powershell
net stop MongoDB
```

12) Notas y buenas prácticas
- En producción cambia la contraseña por defecto del admin y la clave JWT.
- Usa HTTPS en producción.
- Revisa `Program.cs` para opciones de seed y configuración adicional.

Si quieres, puedo:
- Añadir un script PowerShell (por ejemplo `start-all.ps1`) que inicie MongoDB (si lo gestionas localmente) y el backend automáticamente.
- Crear instrucciones para desplegar en Docker o en un servidor.

---
Si quieres que genere el script `start-all.ps1` o un archivo README más corto para incluir en el repositorio, dime y lo creo. 