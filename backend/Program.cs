using MongoDB.Driver;
using TutoriasDeClases.Interfaces;
using TutoriasDeClases.Repositories;
using TutoriasDeClases.Repositorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TutoriasDeClases.Modelos;
using System.IO;
using Microsoft.Extensions.FileProviders;
using MongoDB.Bson;

var builder = WebApplication.CreateBuilder(args);

// ==========================================================
// 1. CONFIGURACIÓN DE SERVICIOS
// ==========================================================

// Agregar controladores
builder.Services.AddControllers();

// Configurar MongoDB como repositorio de usuarios (persistente)
var mongoConnStr = builder.Configuration.GetConnectionString("MongoDbConnection") ?? "mongodb://localhost:27017";
var mongoDbName = builder.Configuration.GetValue<string>("Mongo:Database") ?? "EduMentor";
try
{
    var mongoClient = new MongoClient(mongoConnStr);
    var mongoDatabase = mongoClient.GetDatabase(mongoDbName);
    var usuariosCollection = mongoDatabase.GetCollection<Usuario>("Usuarios");
    builder.Services.AddSingleton<IUsuarioRepository>(sp => new MongoUsuarioRepository(usuariosCollection));
    // Registrar colecciones de tienda y reservas
    var serviciosCollection = mongoDatabase.GetCollection<Servicio>("Servicios");
    var reservasCollection = mongoDatabase.GetCollection<Reserva>("Reservas");
    builder.Services.AddSingleton(serviciosCollection);
    builder.Services.AddSingleton(reservasCollection);
    
    // Registrar repositorio de Cursos
    builder.Services.AddSingleton<IMongoClient>(mongoClient);
    builder.Services.AddScoped<ICursoRepository, CursoRepository>();
    
    // Registrar servicios de negocio (SRP - Single Responsibility Principle)
    builder.Services.AddScoped<TutoriasDeClases.Interfaces.ICursoService, TutoriasDeClases.Servicios.CursoService>();
    
    Console.WriteLine($"✓ Usando MongoUsuarioRepository (DB={mongoDbName}) -> {mongoConnStr}");
    Console.WriteLine($"✓ Usando CursoRepository para gestión de cursos");
    Console.WriteLine($"✓ Servicios de negocio registrados (CursoService)");
}
catch (Exception ex)
{
    // Fallback a InMemory si hay problema con Mongo
    builder.Services.AddSingleton<IUsuarioRepository, InMemoryUsuarioRepository>();
    Console.WriteLine("! No se pudo conectar a MongoDB, usando InMemoryUsuarioRepository. Detalle: " + ex.Message);
}

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configurar JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection.GetValue<string>("Key") ?? "cambiar_esta_clave_por_una_mas_segura_en_produccion_!";
var jwtIssuer = jwtSection.GetValue<string>("Issuer") ?? "EduMentor";
var jwtAudience = jwtSection.GetValue<string>("Audience") ?? "EduMentorUsers";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
    // Permitir que SignalR pase token por query string en WebSocket
    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notifications"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// Registrar SignalR
builder.Services.AddSignalR();

// ==========================================================
// 2. CONFIGURACIÓN DEL PIPELINE HTTP
// ==========================================================
try
{
    // Forzar entorno Development para diagnóstico
    builder.Environment.EnvironmentName = "Development";
    var app = builder.Build();

// ==========================================================
// SEEDING: Crear administrador por defecto si no existe
// ==========================================================
try
{
    var enableDefaultAdminConfig = app.Configuration.GetValue<bool?>("EnableDefaultAdmin");
    var enableDefaultAdmin = enableDefaultAdminConfig ?? app.Environment.IsDevelopment();
    if (enableDefaultAdmin)
    {
        var usuarioRepo = app.Services.GetRequiredService<IUsuarioRepository>();
        var adminExistente = await usuarioRepo.FindByEmailAsync("admin@edumentor.local");
        if (adminExistente == null)
        {
            var adminUser = new Usuario
            {
                Nombre = "Administrador",
                Apellido = "Sistema",
                Correo = "admin@edumentor.local",
                Edad = 0,
                Especializacion = "Administración",
                Rol = "Admin",
                ContrasenaHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                FechaCreacion = DateTime.UtcNow
            };
            await usuarioRepo.AddAsync(adminUser);
            Console.WriteLine("✓ Usuario administrador creado: admin@edumentor.local / password: admin");
        }
        else
        {
            Console.WriteLine("✓ Usuario administrador ya existe.");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Error al crear admin por defecto: {ex.Message}");
}

if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

// Servir archivos estáticos desde la carpeta principal del frontend ('frontend' o 'carpeta')
// Intentar primero en ../frontend, luego en ../carpeta (relativo a ContentRootPath)
string staticFilesPath = null;

// Opción 1: ../frontend (relativo a TutoriasDeClasesbackend)
var frontendPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "frontend"));
if (Directory.Exists(frontendPath))
{
    staticFilesPath = frontendPath;
    Console.WriteLine($"✓ Usando frontend estático: {frontendPath}");
}

// Opción 2: ../carpeta (si frontend no existe)
if (staticFilesPath == null)
{
    var carpetaPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "carpeta"));
    if (Directory.Exists(carpetaPath))
    {
        staticFilesPath = carpetaPath;
        Console.WriteLine($"✓ Usando carpeta estática: {carpetaPath}");
    }
}

// Servir archivos si encontramos una carpeta
if (staticFilesPath != null && Directory.Exists(staticFilesPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(staticFilesPath),
        RequestPath = string.Empty // servir en la raíz
    });
}
else
{
    Console.WriteLine($"⚠️ No se encontró carpeta estática. Buscadas: ../carpeta o ../frontend");
}

app.UseCors("AllowAll");
app.UseAuthentication();
// app.UseHttpsRedirection(); 

app.UseAuthorization();
app.MapControllers(); // Mapea /api/ControladorDeSesion/register, etc.

// Mapear hub de notificaciones
app.MapHub<TutoriasDeClases.Hubs.NotificationHub>("/hubs/notifications");

// Endpoint de prueba para verificar que la app está corriendo
app.MapGet("/api/ping", () => Results.Ok(new { message = "Backend conectado", timestamp = DateTime.UtcNow }));


    Console.WriteLine("=".PadRight(60, '='));
    Console.WriteLine($"🚀 EduMentor Backend iniciando... (ASPNETCORE_ENVIRONMENT={builder.Environment.EnvironmentName})");
    Console.WriteLine("=".PadRight(60, '='));

    try
    {
        Console.WriteLine("==> Antes de app.Run()");
        // Ejecutar la aplicación
        app.Run();
        Console.WriteLine("==> Después de app.Run() (esto no debería verse)");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ EXCEPCION en app.Run(): {ex.GetType().Name}: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
        }
        throw;
    }

    Console.WriteLine("==> FIN DEL MAIN (esto no debería verse)");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ EXCEPCION GLOBAL: {ex.GetType().Name}: {ex.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
    }
    throw;
}
