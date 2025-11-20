# Script pour voir TOUS les logs Godot (sans filtre de mots-cles)
# Utile pour deboguer quand on ne voit rien
# Usage: .\view_all_godot_logs.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Tous les logs Godot (sans filtre)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verifier qu'ADB est disponible
$adbPath = Get-Command adb -ErrorAction SilentlyContinue
if (-not $adbPath) {
    Write-Host "ERREUR: ADB non trouve!" -ForegroundColor Red
    exit 1
}

# Verifier qu'un appareil est connecte
$devices = & adb devices | Select-Object -Skip 1 | Where-Object { $_ -match "device$" }
if (-not $devices) {
    Write-Host "ERREUR: Aucun appareil Android connecte!" -ForegroundColor Red
    exit 1
}

$packageName = "com.company.mygodotgame"

# Obtenir le PID du processus Godot
Write-Host "Recherche du processus Godot..." -ForegroundColor Yellow
$godotPid = $null
try {
    $pidOutput = adb shell "pidof $packageName" 2>$null
    if ($pidOutput -and $pidOutput -match '^\d+$') {
        $godotPid = $pidOutput.Trim()
        Write-Host "OK Processus Godot trouve (PID: $godotPid)" -ForegroundColor Green
    } else {
        Write-Host "ERREUR: Processus Godot non trouve" -ForegroundColor Red
        Write-Host "  Le jeu n'est peut-etre pas lance. Lancez-le depuis l'app MAUI." -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Affichage de tous les logs Android (tres verbeux)..." -ForegroundColor Yellow
        Write-Host "Appuyez sur Ctrl+C pour arreter" -ForegroundColor Yellow
        Write-Host ""
        adb logcat | Select-String -Pattern "$packageName" -CaseSensitive:$false
        exit
    }
} catch {
    Write-Host "ERREUR lors de la recherche du processus: $_" -ForegroundColor Red
    exit 1
}

# Nettoyer les logs precedents
Write-Host "Nettoyage des logs precedents..." -ForegroundColor Yellow
adb logcat -c

Write-Host ""
Write-Host "Affichage de TOUS les logs du processus Godot (PID: $godotPid)..." -ForegroundColor Green
Write-Host "Appuyez sur Ctrl+C pour arreter" -ForegroundColor Yellow
Write-Host ""

# Afficher tous les logs du processus Godot
adb logcat --pid=$godotPid
