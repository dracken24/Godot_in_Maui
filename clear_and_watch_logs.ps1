# Script pour nettoyer les logs et surveiller en temps reel
# Usage: .\clear_and_watch_logs.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Nettoyage et surveillance des logs" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$logFile = "/sdcard/Download/godot_logs.txt"

Write-Host "[1] Nettoyage du fichier de log externe..." -ForegroundColor Yellow
adb shell "echo '' > `"$logFile`" 2>/dev/null && echo 'OK' || echo 'ERREUR'"
Write-Host "OK Fichier nettoye" -ForegroundColor Green
Write-Host ""

Write-Host "[2] Surveillance en temps reel..." -ForegroundColor Yellow
Write-Host "LANCEZ LE JEU DEPUIS L'APP MAUI MAINTENANT!" -ForegroundColor Cyan
Write-Host "Appuyez sur Ctrl+C pour arreter" -ForegroundColor Yellow
Write-Host ""

# Surveiller le fichier
adb shell "tail -f `"$logFile`" 2>/dev/null || echo 'En attente de la creation du fichier...'"

