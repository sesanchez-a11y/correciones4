#!/usr/bin/env PowerShell

# ============================================================================
# Script para Iniciar Backend - EduMentor
# ============================================================================
# Este script inicia el backend ASP.NET Core en el puerto 5000
# ============================================================================

param(
    [switch]$NoWait = $false
)

# Configurar ubicaciones
$projectPath = "c:\tareas\PROYECTO SOFTWARE2\TutoriasDeClasesbackend"
$backend = "TutoriasDeClasesbackend"

Write-Host ""
Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║       🚀 Iniciando Backend - EduMentor                    ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Verificar que la carpeta existe
if (-not (Test-Path $projectPath)) {
    Write-Host "❌ ERROR: No se encontró la carpeta del backend en:" -ForegroundColor Red
    Write-Host "   $projectPath" -ForegroundColor Red
    Write-Host ""
    exit 1
}

Write-Host "✓ Ubicación del proyecto: $projectPath" -ForegroundColor Green
Write-Host ""

# Verificar que .NET está instalado
$dotnetVersion = dotnet --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ ERROR: .NET no está instalado o no está en PATH" -ForegroundColor Red
    Write-Host "   Descarga .NET 9.0 desde: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    Write-Host ""
    exit 1
}

Write-Host "✓ .NET versión instalada: $dotnetVersion" -ForegroundColor Green
Write-Host ""

# Cambiar a la carpeta del proyecto
Push-Location $projectPath

Write-Host "Iniciando: dotnet run" -ForegroundColor Cyan
Write-Host "═════════════════════════════════════════════════════════════" -ForegroundColor Gray
Write-Host ""

# Iniciar el backend
dotnet run

$exitCode = $LASTEXITCODE

Write-Host ""
Write-Host "═════════════════════════════════════════════════════════════" -ForegroundColor Gray

if ($exitCode -eq 0) {
    Write-Host "✓ Backend terminó correctamente" -ForegroundColor Green
} else {
    Write-Host "❌ Backend finalizó con error (código: $exitCode)" -ForegroundColor Red
}

Write-Host ""
Write-Host "Para probar la solución:" -ForegroundColor Yellow
Write-Host "  1. El backend debe estar escuchando en http://localhost:5000" -ForegroundColor White
Write-Host "  2. Abre en navegador: file:///c:/tareas/PROYECTO%20SOFTWARE2/frontend/archivoshtml/reseccion.html" -ForegroundColor White
Write-Host "  3. Inicia sesión" -ForegroundColor White
Write-Host "  4. Deberías ver tu perfil sin que desaparezca" -ForegroundColor White
Write-Host ""

Pop-Location

if (-not $NoWait) {
    Write-Host "Presiona una tecla para cerrar..." -ForegroundColor Gray
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}

exit $exitCode
