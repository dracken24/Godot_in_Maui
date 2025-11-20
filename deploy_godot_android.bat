@echo off
REM Script batch pour déployer l'APK Godot sur Android
REM Double-cliquez sur ce fichier pour lancer le déploiement

powershell.exe -ExecutionPolicy Bypass -File "%~dp0deploy_godot_android.ps1"

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo Appuyez sur une touche pour fermer...
    pause >nul
)

