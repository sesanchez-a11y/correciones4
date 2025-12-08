using MongoDB.Driver;
using TutoriasDeClases.Interfaces;
using TutoriasDeClases.Repositories;
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
    Console.WriteLine($"✓ Usando MongoUsuarioRepository (DB={mongoDbName}) -> {mongoConnStr}");
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
});

// ==========================================================
// 2. CONFIGURACIÓN DEL PIPELINE HTTP
// ==========================================================
var app = builder.Build();

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

// Endpoint de prueba para verificar que la app está corriendo
app.MapGet("/api/ping", () => Results.Ok(new { message = "Backend conectado", timestamp = DateTime.UtcNow }));

Console.WriteLine("=".PadRight(60, '='));
Console.WriteLine("🚀 EduMentor Backend iniciando...");
Console.WriteLine("=".PadRight(60, '='));

try
{
    // Ejecutar la aplicación
    app.Run();
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

// Este código nunca se ejecutará porque app.Run() es bloqueante
Console.WriteLine("Backend ha finalizado");