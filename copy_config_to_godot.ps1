# Script pour copier le fichier de configuration dans le user data dir de l'app Godot
# Usage: .\copy_config_to_godot.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Copie du fichier de configuration" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$configFile = "/sdcard/Download/platformgame_config.json"
$godotUserDataDir = "/data/data/com.company.mygodotgame/files"
$targetFile = "$godotUserDataDir/platformgame_config.json"

Write-Host "[1] Verification du fichier source..." -ForegroundColor Yellow
$exists = adb shell "test -f `"$configFile`" && echo 'EXISTS' || echo 'NOT_EXISTS'"
if ($exists -match "NOT_EXISTS") {
    Write-Host "ERREUR: Fichier source non trouve: $configFile" -ForegroundColor Red
    Write-Host "  Le fichier doit etre cree par l'app MAUI lors du lancement du jeu." -ForegroundColor Yellow
    exit 1
}
Write-Host "OK Fichier source trouve" -ForegroundColor Green
Write-Host ""

Write-Host "[2] Lecture du contenu..." -ForegroundColor Yellow
$content = adb shell "cat `"$configFile`""
Write-Host "OK Contenu:" -ForegroundColor Green
Write-Host $content -ForegroundColor Gray
Write-Host ""

Write-Host "[3] Copie dans le user data dir de l'app Godot..." -ForegroundColor Yellow
Write-Host "  (Cela ne fonctionnera que si l'APK est en mode debuggable)" -ForegroundColor Gray
Write-Host ""

# Encoder le contenu en base64 pour preserver le format JSON
$bytes = [System.Text.Encoding]::UTF8.GetBytes($content)
$base64 = [Convert]::ToBase64String($bytes)

# Copier le contenu encode en base64, puis le decoder dans le shell Android
$result = adb shell "run-as com.company.mygodotgame sh -c 'echo `"$base64`" | base64 -d > $targetFile' 2>&1"
if ($result -match "package not debuggable") {
    Write-Host "ERREUR: L'APK n'est pas en mode debuggable." -ForegroundColor Red
    Write-Host ""
    Write-Host "Solutions:" -ForegroundColor Yellow
    Write-Host "  1. Activer le mode debuggable dans Godot:" -ForegroundColor Cyan
    Write-Host "     - Projet -> Exporter -> Android -> Options" -ForegroundColor Cyan
    Write-Host "     - Custom Template -> Debug: laisser vide" -ForegroundColor Cyan
    Write-Host "     - Reexporter l'APK" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  2. Alternative: Utiliser adb push (necessite root):" -ForegroundColor Cyan
    Write-Host "     adb push <fichier_local> $targetFile" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  3. Pour l'instant, le fichier reste dans /sdcard/Download/" -ForegroundColor Yellow
    Write-Host "     Mais Godot ne peut pas le lire a cause des permissions Android." -ForegroundColor Yellow
} elseif ($result) {
    Write-Host "ATTENTION: Erreur lors de la copie: $result" -ForegroundColor Yellow
} else {
    Write-Host "OK Fichier copie!" -ForegroundColor Green
    Write-Host ""
    Write-Host "[4] Verification..." -ForegroundColor Yellow
    $verify = adb shell "run-as com.company.mygodotgame cat $targetFile 2>&1"
    if ($verify -and $verify -notmatch "package not debuggable" -and $verify -notmatch "No such file") {
        Write-Host "OK Fichier verifie:" -ForegroundColor Green
        Write-Host $verify -ForegroundColor Gray
    } else {
        Write-Host "ATTENTION: Impossible de verifier le fichier" -ForegroundColor Yellow
        Write-Host "  Reponse: $verify" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan

