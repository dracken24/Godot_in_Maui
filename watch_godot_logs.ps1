# Script pour surveiller le fichier de log Godot en temps reel
# Usage: .\watch_godot_logs.ps1

Write-Host "========================================"
Write-Host "  Surveillance des logs Godot"
Write-Host "========================================"
Write-Host ""
Write-Host "Appuyez sur Ctrl+C pour arreter"
Write-Host ""

# Essayer plusieurs chemins possibles
# Le user data dir est garanti de fonctionner
$userDataDir = "/data/data/com.company.mygodotgame/files"
$logPaths = @(
    "$userDataDir/godot_logs.txt",  # User data dir (garanti de fonctionner)
    "/sdcard/Download/godot_logs.txt",
    "/storage/emulated/0/Download/godot_logs.txt"
)

Write-Host "[1] Recherche du fichier de log..."
$foundPath = $null
foreach ($logPath in $logPaths) {
    $exists = adb shell "test -f `"$logPath`" && echo 'EXISTS' || echo 'NOT_EXISTS'"
    if ($exists -match "EXISTS") {
        $foundPath = $logPath
        Write-Host "OK Fichier trouve: $foundPath" -ForegroundColor Green
        break
    }
}

if (-not $foundPath) {
    Write-Host "ATTENTION: Aucun fichier de log trouve." -ForegroundColor Yellow
    Write-Host "Utilisation du premier chemin par defaut: $($logPaths[0])" -ForegroundColor Yellow
    $foundPath = $logPaths[0]
    Write-Host ""
    Write-Host "Le fichier sera cree automatiquement quand le jeu ecrira des logs."
    Write-Host ""
}

# Nettoyer le fichier de log si demande
Write-Host "Voulez-vous nettoyer le fichier de log avant de commencer? (O/N)"
$response = Read-Host
if ($response -eq "O" -or $response -eq "o") {
    Write-Host "Nettoyage du fichier de log..."
    adb shell "echo '' > `"$foundPath`""
    Write-Host "OK"
    Write-Host ""
}

Write-Host "Surveillance du fichier de log en temps reel..."
Write-Host "Chemin: $foundPath"
Write-Host "Lancez le jeu depuis l'app MAUI pour voir les logs."
Write-Host "Appuyez sur Ctrl+C pour arreter"
Write-Host ""

# Surveiller le fichier
# Si c'est dans le user data dir, utiliser le fichier externe
if ($foundPath -like "/data/data/*") {
    Write-Host "ATTENTION: Le fichier est dans le user data dir." -ForegroundColor Yellow
    Write-Host "Surveillance du fichier externe a la place..." -ForegroundColor Yellow
    Write-Host ""
    
    $externalPath = "/sdcard/Download/godot_logs.txt"
    Write-Host "Surveillance de: $externalPath" -ForegroundColor Cyan
    Write-Host "Lancez le jeu depuis l'app MAUI pour voir les logs." -ForegroundColor Yellow
    Write-Host "Appuyez sur Ctrl+C pour arreter" -ForegroundColor Yellow
    Write-Host ""
    
    # Surveiller le fichier externe
    adb shell "tail -f `"$externalPath`" 2>/dev/null || echo 'Fichier non trouve, attente de sa creation...'"
} else {
    Write-Host "Surveillance de: $foundPath" -ForegroundColor Cyan
    Write-Host "Lancez le jeu depuis l'app MAUI pour voir les logs." -ForegroundColor Yellow
    Write-Host "Appuyez sur Ctrl+C pour arreter" -ForegroundColor Yellow
    Write-Host ""
    adb shell "tail -f `"$foundPath`""
}

