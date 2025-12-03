# Script pour voir les logs de Godot depuis Android
# Utilise logcat pour filtrer les logs de l'application Godot

Write-Host "=== Logs Godot (Android) ===" -ForegroundColor Yellow
Write-Host "Appuyez sur Ctrl+C pour arrêter" -ForegroundColor Cyan
Write-Host ""

# Vérifier qu'ADB est disponible
$adbCheck = adb version 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERREUR: ADB n'est pas disponible" -ForegroundColor Red
    Write-Host "Installez Android SDK Platform Tools" -ForegroundColor Yellow
    exit 1
}

# Vérifier qu'un appareil est connecté
$devices = adb devices | Select-String "device$"
if (-not $devices) {
    Write-Host "ERREUR: Aucun appareil Android connecté" -ForegroundColor Red
    Write-Host "Connectez votre appareil et activez le débogage USB" -ForegroundColor Yellow
    exit 1
}

Write-Host "Appareil connecté" -ForegroundColor Green
Write-Host ""

# Options de filtrage
Write-Host "Options de filtrage:" -ForegroundColor Cyan
Write-Host "1. Logs de l'app Godot uniquement (com.company.mygodotgame)" -ForegroundColor White
Write-Host "2. Logs HTTP/Network (requêtes API)" -ForegroundColor White
Write-Host "3. Logs de l'app MAUI (com.companyname.platformgame)" -ForegroundColor White
Write-Host "4. Tous les logs (non filtré)" -ForegroundColor White
Write-Host "5. Logs d'erreurs uniquement" -ForegroundColor White
Write-Host ""

$choice = Read-Host "Choisissez une option (1-5, défaut: 2)"

switch ($choice) {
    "1" {
        Write-Host "Filtrage: Logs de l'app Godot uniquement" -ForegroundColor Yellow
        Write-Host ""
        adb logcat -c  # Clear logs
        adb logcat | Select-String -Pattern "com\.company\.mygodotgame|Godot|godot" -CaseSensitive:$false
    }
    "2" {
        Write-Host "Filtrage: Logs HTTP/Network (requêtes API)" -ForegroundColor Yellow
        Write-Host ""
        adb logcat -c  # Clear logs
        adb logcat | Select-String -Pattern "HTTP|http|API|api|POST|GET|Network|Socket|Connection" -CaseSensitive:$false
    }
    "3" {
        Write-Host "Filtrage: Logs de l'app MAUI" -ForegroundColor Yellow
        Write-Host ""
        adb logcat -c  # Clear logs
        adb logcat | Select-String -Pattern "com\.companyname\.platformgame|PlatformGame|monodroid" -CaseSensitive:$false
    }
    "4" {
        Write-Host "Affichage: Tous les logs (non filtré)" -ForegroundColor Yellow
        Write-Host ""
        adb logcat -c  # Clear logs
        adb logcat
    }
    "5" {
        Write-Host "Filtrage: Erreurs uniquement" -ForegroundColor Yellow
        Write-Host ""
        adb logcat -c  # Clear logs
        adb logcat *:E | Select-String -Pattern "Error|Exception|Failed|Socket|Connection" -CaseSensitive:$false
    }
    default {
        Write-Host "Filtrage: Logs HTTP/Network (par défaut)" -ForegroundColor Yellow
        Write-Host ""
        adb logcat -c  # Clear logs
        adb logcat | Select-String -Pattern "HTTP|http|API|api|POST|GET|Network|Socket|Connection|GameResult|CreateResult" -CaseSensitive:$false
    }
}

