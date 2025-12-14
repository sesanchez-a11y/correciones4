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
$projectPath = "backend"
$backend = "backend"

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

if (-not $NoWait) {
    dotnet run
} else {
    Start-Process dotnet -ArgumentList "run" -NoNewWindow
}

Pop-Location

Write-Host ""
Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║       ✓ Backend iniciado - EduMentor                     ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""
Write-Host "Accesible en: http://localhost:5000" -ForegroundColor Yellow
Write-Host ""
