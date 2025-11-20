# Script pour voir TOUS les logs Godot possibles
# Inclut les logs system, stderr, stdout, et tous les tags

Write-Host "========================================"
Write-Host "  Recherche de TOUS les logs Godot"
Write-Host "========================================"
Write-Host ""

# Trouver le PID du processus Godot
Write-Host "[1] Recherche du processus Godot..."
$godotProcess = adb shell "ps -A | grep mygodotgame" | Select-String "mygodotgame"
if ($godotProcess) {
    $pid = ($godotProcess -split '\s+')[1]
    Write-Host "OK Processus trouve: PID $pid"
} else {
    Write-Host "ATTENTION: Processus Godot non trouve"
    Write-Host "Lancez le jeu depuis l'app MAUI d'abord!"
    Write-Host ""
    Write-Host "Affichage de tous les logs system (sans filtre)..."
    Write-Host "Appuyez sur Ctrl+C pour arreter"
    Write-Host ""
    adb logcat | Select-String -Pattern "godot|Godot|mygodotgame|End|UserEmailManager" -CaseSensitive:$false
    exit
}

Write-Host ""
Write-Host "[2] Affichage de TOUS les logs pour le PID $pid..."
Write-Host "Appuyez sur Ctrl+C pour arreter"
Write-Host ""

# Afficher tous les logs pour ce PID
adb logcat --pid=$pid

