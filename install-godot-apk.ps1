# Script PowerShell pour installer l'APK Godot sur Android
# Usage: .\install-godot-apk.ps1

Write-Host "=== Installation de l'APK Godot ===" -ForegroundColor Cyan

# Vérifier que ADB est disponible
$adbPath = Get-Command adb -ErrorAction SilentlyContinue
if (-not $adbPath) {
    Write-Host "ERREUR: ADB n'est pas trouvé dans le PATH" -ForegroundColor Red
    Write-Host "Assurez-vous que le SDK Android est installé et que ADB est dans votre PATH" -ForegroundColor Yellow
    exit 1
}

# Vérifier les appareils connectés
Write-Host "`nVérification des appareils connectés..." -ForegroundColor Yellow
$devices = adb devices
Write-Host $devices

$deviceCount = (adb devices | Select-String -Pattern "device$" | Measure-Object).Count
if ($deviceCount -eq 0) {
    Write-Host "`nERREUR: Aucun appareil Android connecté ou l'émulateur n'est pas démarré" -ForegroundColor Red
    Write-Host "Assurez-vous que:" -ForegroundColor Yellow
    Write-Host "  1. L'émulateur Android est démarré" -ForegroundColor Yellow
    Write-Host "  2. Ou qu'un appareil physique est connecté avec USB Debugging activé" -ForegroundColor Yellow
    exit 1
}

# Chemin de l'APK
$apkPath = Join-Path $PSScriptRoot "Godot\MyGodotGame.apk"

if (-not (Test-Path $apkPath)) {
    Write-Host "`nERREUR: APK non trouvé à: $apkPath" -ForegroundColor Red
    exit 1
}

Write-Host "`nAPK trouvé: $apkPath" -ForegroundColor Green

# Vérifier l'espace disponible
Write-Host "`nVérification de l'espace disponible..." -ForegroundColor Yellow
$dfOutput = adb shell df -h /data
Write-Host $dfOutput

# Essayer d'installer l'APK
Write-Host "`nInstallation de l'APK..." -ForegroundColor Yellow
$installResult = adb install -r $apkPath 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nSUCCÈS: APK installé avec succès!" -ForegroundColor Green
    
    # Récupérer le nom du package
    Write-Host "`nRécupération du nom du package..." -ForegroundColor Yellow
    $packageInfo = adb shell pm list packages | Select-String -Pattern "godot" -CaseSensitive:$false
    if ($packageInfo) {
        Write-Host "Package trouvé: $packageInfo" -ForegroundColor Green
        $packageName = ($packageInfo -split ':')[1]
        Write-Host "`nNom du package à utiliser dans votre code: $packageName" -ForegroundColor Cyan
    }
} else {
    Write-Host "`nERREUR lors de l'installation:" -ForegroundColor Red
    Write-Host $installResult -ForegroundColor Red
    
    if ($installResult -match "not enough space") {
        Write-Host "`nSOLUTION: L'émulateur n'a pas assez d'espace" -ForegroundColor Yellow
        Write-Host "Essayez de:" -ForegroundColor Yellow
        Write-Host "  1. Désinstaller des applications inutiles: adb shell pm uninstall <package>" -ForegroundColor Yellow
        Write-Host "  2. Augmenter la taille de l'émulateur dans AVD Manager" -ForegroundColor Yellow
        Write-Host "  3. Redémarrer l'émulateur" -ForegroundColor Yellow
    } elseif ($installResult -match "INSTALL_PARSE_FAILED") {
        Write-Host "`nSOLUTION: L'APK semble corrompu ou invalide" -ForegroundColor Yellow
        Write-Host "Vérifiez que l'APK a été compilé correctement depuis Godot" -ForegroundColor Yellow
    }
}

Write-Host "`n=== Fin ===" -ForegroundColor Cyan

