# Script PowerShell pour voir les logs Godot depuis Android
# Usage: .\view_godot_logs.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Logs Godot Android" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Appuyez sur Ctrl+C pour arrêter" -ForegroundColor Yellow
Write-Host ""

# Vérifier qu'ADB est disponible
$adbPath = Get-Command adb -ErrorAction SilentlyContinue
if (-not $adbPath) {
    Write-Host "ERREUR: ADB non trouvé!" -ForegroundColor Red
    Write-Host "Installez Android SDK Platform Tools ou ajoutez ADB au PATH." -ForegroundColor Red
    exit 1
}

# Vérifier qu'un appareil est connecté
$devices = & adb devices | Select-Object -Skip 1 | Where-Object { $_ -match "device$" }
if (-not $devices) {
    Write-Host "ERREUR: Aucun appareil Android connecté!" -ForegroundColor Red
    exit 1
}

$packageName = "com.company.mygodotgame"

# Nettoyer les logs précédents
Write-Host "Nettoyage des logs précédents..." -ForegroundColor Yellow
adb logcat -c

Write-Host "Affichage des logs Godot en temps réel..." -ForegroundColor Green
Write-Host "Recherche des mots-clés: UserEmailManager, End, SaveScore, Godot" -ForegroundColor Cyan
Write-Host "Package: $packageName" -ForegroundColor Cyan
Write-Host ""
Write-Host "ASTUCE: Lancez le jeu depuis l'app MAUI, puis regardez les logs ci-dessous" -ForegroundColor Yellow
Write-Host ""

# Afficher les logs avec filtres
# On filtre pour voir les logs du package Godot et les messages importants
adb logcat | Select-String -Pattern "UserEmailManager|End|SaveScore|Godot|$packageName|platformgame_config" -CaseSensitive:$false

