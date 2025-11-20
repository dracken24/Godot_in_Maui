# Script pour capturer les logs Godot de toutes les manieres possibles

Write-Host "========================================"
Write-Host "  Capture des logs Godot"
Write-Host "========================================"
Write-Host ""

# Vider les logs
Write-Host "[1] Vidage des logs..."
adb logcat -c
Write-Host "OK"
Write-Host ""

# Attendre que l'utilisateur lance le jeu
Write-Host "[2] LANCEZ LE JEU DEPUIS L'APP MAUI MAINTENANT!"
Write-Host "Appuyez sur Entree quand le jeu est lance..."
Read-Host
Write-Host ""

# Trouver le PID du processus Godot
Write-Host "[3] Recherche du processus Godot..."
Start-Sleep -Seconds 2
$godotProcess = adb shell "ps -A | grep mygodotgame" | Select-String "mygodotgame"
if ($godotProcess) {
    $pid = ($godotProcess -split '\s+')[1]
    Write-Host "OK Processus trouve: PID $pid"
} else {
    Write-Host "ERREUR: Processus Godot non trouve"
    Write-Host "Le jeu n'est peut-etre pas lance."
    exit 1
}
Write-Host ""

# Methode 1: Logs par PID
Write-Host "[4] Methode 1: Logs par PID ($pid)..."
Write-Host "Appuyez sur Ctrl+C pour arreter"
Write-Host ""
adb logcat --pid=$pid

