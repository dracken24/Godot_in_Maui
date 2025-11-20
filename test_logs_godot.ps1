# Script pour tester si les logs Godot fonctionnent
# Usage: .\test_logs_godot.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Test des logs Godot" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$logFile = "/sdcard/Download/godot_logs.txt"

Write-Host "[1] Verification du fichier de log..." -ForegroundColor Yellow
$fileExists = adb shell "test -f `"$logFile`" && echo 'EXISTS' || echo 'NOT_EXISTS'"
if ($fileExists -match "EXISTS") {
    $size = adb shell "stat -c%s `"$logFile`" 2>/dev/null || echo '0'"
    $size = $size.Trim()
    $lastModified = adb shell "stat -c %y `"$logFile`" 2>/dev/null"
    Write-Host "OK Fichier existe (taille: $size octets)" -ForegroundColor Green
    Write-Host "   Derniere modification: $lastModified" -ForegroundColor Gray
} else {
    Write-Host "ATTENTION: Fichier de log non trouve!" -ForegroundColor Yellow
    Write-Host "  Le fichier sera cree lors du premier lancement du jeu." -ForegroundColor Yellow
}
Write-Host ""

Write-Host "[2] Verification du processus Godot..." -ForegroundColor Yellow
$pidOutput = adb shell "pidof com.company.mygodotgame" 2>$null
if ($pidOutput -and $pidOutput -match '^\d+$') {
    $processId = $pidOutput.Trim()
    Write-Host "OK Processus Godot en cours d'execution (PID: $processId)" -ForegroundColor Green
} else {
    Write-Host "ATTENTION: Processus Godot non trouve" -ForegroundColor Yellow
    Write-Host "  Le jeu n'est pas lance. Lancez-le depuis l'app MAUI." -ForegroundColor Yellow
}
Write-Host ""

Write-Host "[3] Affichage des 10 dernieres lignes du fichier de log..." -ForegroundColor Yellow
Write-Host ""
$content = adb shell "tail -10 `"$logFile`" 2>/dev/null"
if ($content -and $content.Trim().Length -gt 0) {
    Write-Host $content
} else {
    Write-Host "(fichier vide ou erreur)" -ForegroundColor Gray
}
Write-Host ""

Write-Host "[4] Surveillance en temps reel (appuyez sur Ctrl+C pour arreter)..." -ForegroundColor Yellow
Write-Host "Lancez le jeu depuis l'app MAUI pour voir les nouveaux logs." -ForegroundColor Cyan
Write-Host ""

# Surveiller le fichier
adb shell "tail -f `"$logFile`" 2>/dev/null || echo 'En attente de la creation du fichier...'"

