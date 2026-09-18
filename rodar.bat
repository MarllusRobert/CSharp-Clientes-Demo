@echo off
setlocal
cd /d "%~dp0"

where dotnet >nul 2>&1
if errorlevel 1 (
  echo .NET SDK nao encontrado. Instale: https://dotnet.microsoft.com/download
  exit /b 1
)

echo Compilando ClientesDemo...
dotnet build "ClientesDemo\ClientesDemo.csproj" -c Release
if errorlevel 1 (
  echo Falha na compilacao.
  exit /b 1
)

echo Iniciando aplicativo...
start "" "ClientesDemo\bin\Release\net8.0-windows\ClientesDemo.exe"
endlocal
