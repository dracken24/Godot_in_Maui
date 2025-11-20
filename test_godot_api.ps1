# Script pour tester la communication Godot -> API
# Ce script lance le jeu depuis l'app MAUI et affiche les logs en temps réel
# Usage: .\test_godot_api.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Test Communication Godot -> API" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Vérifier qu'ADB est disponible
$adbPath = Get-Command adb -ErrorAction SilentlyContinue
if (-not $adbPath) {
    Write-Host "ERREUR: ADB non trouvé!" -ForegroundColor Red
    exit 1
}

# Vérifier qu'un appareil est connecté
$devices = & adb devices | Select-Object -Skip 1 | Where-Object { $_ -match "device$" }
if (-not $devices) {
    Write-Host "ERREUR: Aucun appareil Android connecté!" -ForegroundColor Red
    exit 1
}

Write-Host "Instructions:" -ForegroundColor Yellow
Write-Host "1. Assurez-vous que l'app MAUI est installée et que vous êtes connecté" -ForegroundColor White
Write-Host "2. Assurez-vous que l'API backend est en cours d'exécution sur http://10.0.0.49:5000" -ForegroundColor White
Write-Host "3. Les logs Godot seront affichés ci-dessous en temps réel" -ForegroundColor White
Write-Host "4. Lancez le jeu depuis l'app MAUI et terminez un niveau" -ForegroundColor White
Write-Host ""
Write-Host "Appuyez sur Entrée pour commencer à afficher les logs..." -ForegroundColor Cyan
Read-Host

# Nettoyer les logs précédents
Write-Host "Nettoyage des logs précédents..." -ForegroundColor Yellow
adb logcat -c

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Affichage des logs en temps réel" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Recherche des messages importants:" -ForegroundColor Cyan
Write-Host "  - [UserEmailManager] : Chargement de l'email et de l'API" -ForegroundColor White
Write-Host "  - [End] : Détection de la fin du niveau" -ForegroundColor White
Write-Host "  - SaveScore : Envoi du score à l'API" -ForegroundColor White
Write-Host ""
Write-Host "Appuyez sur Ctrl+C pour arrêter" -ForegroundColor Yellow
Write-Host ""

# Filtrer les logs pour voir seulement les messages pertinents
# On cherche les logs du package Godot et les messages de nos scripts
Write-Host "Filtrage des logs pour:" -ForegroundColor Cyan
Write-Host "  - UserEmailManager" -ForegroundColor White
Write-Host "  - End" -ForegroundColor White
Write-Host "  - SaveScore" -ForegroundColor White
Write-Host "  - Godot" -ForegroundColor White
Write-Host "  - com.company.mygodotgame" -ForegroundColor White
Write-Host ""

# Utiliser adb logcat avec des filtres plus spécifiques
# Format: adb logcat -s TAG:LEVEL
# On filtre pour voir tous les logs du package Godot
$packageName = "com.company.mygodotgame"

# Obtenir le PID du processus Godot si disponible
$godotPid = $null
try {
    $pidOutput = adb shell "pidof $packageName" 2>$null
    if ($pidOutput -and $pidOutput -match '^\d+$') {
        $godotPid = $pidOutput.Trim()
        Write-Host "Processus Godot trouvé (PID: $godotPid)" -ForegroundColor Green
    }
} catch {
    Write-Host "Processus Godot non trouvé (le jeu n'est peut-être pas lancé)" -ForegroundColor Yellow
}

# Afficher les logs avec filtres
if ($godotPid) {
    Write-Host "Affichage des logs pour le PID $godotPid..." -ForegroundColor Green
    adb logcat --pid=$godotPid | Select-String -Pattern "UserEmailManager|End|SaveScore|Godot|platformgame_config|api/gameresults|ERROR|WARN" -CaseSensitive:$false
} else {
    Write-Host "Affichage de tous les logs avec filtres..." -ForegroundColor Green
    Write-Host "(Si aucun log n'apparaît, lancez le jeu depuis l'app MAUI)" -ForegroundColor Yellow
    Write-Host ""
    # Filtrer par package et mots-clés
    adb logcat | Select-String -Pattern "UserEmailManager|End|SaveScore|Godot|$packageName|platformgame_config|api/gameresults" -CaseSensitive:$false
}

