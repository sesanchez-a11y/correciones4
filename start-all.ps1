<#
start-all.ps1
Inicia: MongoDB service (si existe), el backend (.NET) y el servidor estático (Python http.server).
Uso: Ejecutar desde PowerShell: ./scripts/start-all.ps1
#>
Set-StrictMode -Version Latest

$root = Split-Path -Parent $MyInvocation.MyCommand.Definition
Write-Host "Workspace root: $root"

Write-Host "\n1) Comprobando servicio MongoDB..."
$svc = Get-Service -Name MongoDB -ErrorAction SilentlyContinue
if ($null -eq $svc) {
    Write-Warning "Servicio 'MongoDB' no encontrado. Comprueba la instalación de MongoDB."
} elseif ($svc.Status -ne 'Running') {
    Write-Host "Iniciando servicio MongoDB..."
    try { Start-Service MongoDB -ErrorAction Stop; Start-Sleep -Seconds 2; Write-Host 'MongoDB iniciado.' } catch { Write-Warning "No se pudo iniciar MongoDB: $_" }
} else {
    Write-Host "MongoDB ya está en ejecución."
}

Write-Host "\n2) Iniciando backend (.NET) en una nueva ventana de PowerShell..."
$backendDir = Join-Path $root 'backend'
if (-not (Test-Path $backendDir)) { Write-Warning "No se encontró el directorio del backend: $backendDir" } else {
    $psCommand = "$env:ASPNETCORE_ENVIRONMENT='Development'; $env:EnableDefaultAdmin='true'; cd '$backendDir'; dotnet run"
    Start-Process -FilePath 'powershell.exe' -ArgumentList '-NoExit','-Command', $psCommand -WorkingDirectory $backendDir
    Write-Host "Backend: iniciado (ver ventana de PowerShell separada)."
}

Write-Host "\n3) Iniciando servidor estático (Python http.server) en una nueva ventana de PowerShell..."
$frontendDir = Join-Path $root 'frontend/archivoshtml'
if (-not (Test-Path $frontendDir)) { Write-Warning "No se encontró el directorio frontend: $frontendDir" } else {
    $psCommand2 = "cd '$frontendDir'; python -m http.server 5500"
    Start-Process -FilePath 'powershell.exe' -ArgumentList '-NoExit','-Command', $psCommand2 -WorkingDirectory $frontendDir
    Write-Host "Servidor estático: iniciado en http://localhost:5500 (ventana separada)."
}

Write-Host "\nListo. Comprueba las ventanas nuevas de PowerShell para los logs." 
