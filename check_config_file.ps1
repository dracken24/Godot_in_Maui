# Script pour verifier si le fichier de configuration existe et est accessible
# Usage: .\check_config_file.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Verification du fichier de configuration" -ForegroundColor Cyan
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

$configPath = "/storage/emulated/0/Documents/platformgame_config.json"

Write-Host "Verification du fichier: $configPath" -ForegroundColor Yellow
Write-Host ""

# Verifier si le fichier existe
Write-Host "[1] Existence du fichier..." -ForegroundColor Cyan
$fileExists = adb shell "test -f '$configPath' && echo 'EXISTS' || echo 'NOT_FOUND'"
if ($fileExists -match "EXISTS") {
    Write-Host "OK Le fichier existe" -ForegroundColor Green
} else {
    Write-Host "ERREUR: Le fichier n'existe pas!" -ForegroundColor Red
    Write-Host "  Le fichier doit etre cree par l'app MAUI lors du lancement du jeu." -ForegroundColor Yellow
    Write-Host "  Relancez le jeu depuis l'app MAUI." -ForegroundColor Yellow
    exit 1
}

# Afficher le contenu du fichier
Write-Host ""
Write-Host "[2] Contenu du fichier..." -ForegroundColor Cyan
$fileContent = adb shell "cat '$configPath'"
Write-Host $fileContent -ForegroundColor White

# Verifier les permissions
Write-Host ""
Write-Host "[3] Permissions du fichier..." -ForegroundColor Cyan
$permissions = adb shell "ls -la '$configPath'"
Write-Host $permissions -ForegroundColor White

# Verifier si l'app Godot peut acceder au fichier
Write-Host ""
Write-Host "[4] Test d'acces depuis l'app Godot..." -ForegroundColor Cyan
$packageName = "com.company.mygodotgame"
$testAccess = adb shell "run-as $packageName cat '$configPath' 2>&1"
if ($testAccess -match "No such file" -or $testAccess -match "Permission denied") {
    Write-Host "ATTENTION: L'app Godot ne peut peut-etre pas acceder au fichier" -ForegroundColor Yellow
    Write-Host "  Reponse: $testAccess" -ForegroundColor Yellow
} else {
    Write-Host "OK L'app Godot peut acceder au fichier" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Verification terminee" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

