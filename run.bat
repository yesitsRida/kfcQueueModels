@echo off
setlocal
cd /d "%~dp0"
if exist "%USERPROFILE%\.dotnet\dotnet.exe" set "PATH=%USERPROFILE%\.dotnet;%PATH%"
dotnet run --project KfcQueueModels.csproj -- %*
