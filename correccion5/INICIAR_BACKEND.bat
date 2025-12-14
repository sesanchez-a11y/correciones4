@echo off
REM Script para iniciar el backend de EduMentor

cd /d "c:\tareas\PROYECTO SOFTWARE2\TutoriasDeClasesbackend"

echo.
echo ===============================================
echo  🚀 Iniciando Backend EduMentor
echo ===============================================
echo.
echo Backend: ASP.NET Core 9.0
echo Ubicación: %CD%
echo.

dotnet run

echo.
echo ===============================================
echo  ✓ Backend iniciado
echo ===============================================
echo.
echo Accesible en: http://localhost:5000
echo.
pause
