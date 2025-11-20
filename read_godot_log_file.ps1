# Script pour lire le fichier de log Godot depuis Android
# Usage: .\read_godot_log_file.ps1

Write-Host "========================================"
Write-Host "  Lecture du fichier de log Godot"
Write-Host "========================================"
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
        Write-Host "OK Fichier trouve: $logPath" -ForegroundColor Green
        break
    }
}

if (-not $foundPath) {
    Write-Host "ATTENTION: Aucun fichier de log trouve dans les chemins suivants:" -ForegroundColor Yellow
    foreach ($logPath in $logPaths) {
        Write-Host "  - $logPath"
    }
    Write-Host ""
    Write-Host "Lancez le jeu depuis l'app MAUI pour generer des logs."
    Write-Host "Surveillance du premier chemin (appuyez sur Ctrl+C pour arreter)..."
    Write-Host ""
    
    # Surveiller le premier chemin
    $logFile = $logPaths[0]
    while ($true) {
        $exists = adb shell "test -f `"$logFile`" && echo 'EXISTS' || echo 'NOT_EXISTS'"
        if ($exists -match "EXISTS") {
            Write-Host "Fichier cree! Affichage du contenu..." -ForegroundColor Green
            Write-Host ""
            $foundPath = $logFile
            break
        }
        Start-Sleep -Seconds 1
    }
}

Write-Host "[2] Lecture du fichier de log ($foundPath)..."
Write-Host ""

# Si c'est dans le user data dir, on ne peut pas le lire directement
# (run-as ne fonctionne que si l'APK est en mode debuggable)
# On lit le fichier externe a la place
if ($foundPath -like "/data/data/*") {
    Write-Host "ATTENTION: Le fichier est dans le user data dir." -ForegroundColor Yellow
    Write-Host "Pour lire ce fichier, l'APK doit etre en mode debuggable." -ForegroundColor Yellow
    Write-Host "Lecture du fichier externe a la place..." -ForegroundColor Yellow
    Write-Host ""
    
    # Essayer de lire le fichier externe
    $externalPath = "/sdcard/Download/godot_logs.txt"
    $content = adb shell "cat `"$externalPath`" 2>/dev/null"
    if ($content -and $content.Trim().Length -gt 0) {
        Write-Host "Contenu du fichier externe ($externalPath):" -ForegroundColor Cyan
        Write-Host $content
    } else {
        Write-Host "(fichier externe vide ou inexistant)" -ForegroundColor Gray
        Write-Host ""
        Write-Host "Le fichier principal est dans: $foundPath" -ForegroundColor Yellow
        Write-Host "Pour le lire, vous devez:" -ForegroundColor Yellow
        Write-Host "  1. Activer le mode debuggable dans les parametres de l'APK Godot" -ForegroundColor Yellow
        Write-Host "  2. Ou utiliser: adb pull $foundPath godot_logs.txt" -ForegroundColor Yellow
    }
} else {
    adb shell "cat `"$foundPath`""
}

Write-Host ""
Write-Host "[3] Pour voir les logs en temps reel, utilisez:"
Write-Host "    adb shell 'tail -f $logFile'"
Write-Host ""

