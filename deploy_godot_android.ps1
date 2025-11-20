# Script PowerShell pour deployer l'APK Godot sur le premier telephone Android connecte
# Usage: .\deploy_godot_android.ps1

param(
    [string]$ApkPath = "",
    [string]$PackageName = "com.company.mygodotgame"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Deploiement APK Godot sur Android" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 1. Verifier que ADB est disponible
Write-Host "[1/5] Verification de ADB..." -ForegroundColor Yellow
$adbPath = Get-Command adb -ErrorAction SilentlyContinue
if (-not $adbPath) {
    # Essayer de trouver ADB dans le SDK Android
    $sdkPath = $env:ANDROID_HOME
    if (-not $sdkPath) {
        $sdkPath = "$env:LOCALAPPDATA\Android\Sdk"
    }
    
    $platformTools = Join-Path $sdkPath "platform-tools\adb.exe"
    if (Test-Path $platformTools) {
        $adbPath = $platformTools
        $env:PATH += ";$(Split-Path $platformTools)"
    } else {
        Write-Host "ERREUR: ADB non trouve!" -ForegroundColor Red
        Write-Host "Installez Android SDK Platform Tools ou ajoutez ADB au PATH." -ForegroundColor Red
        Write-Host "Telechargement: https://developer.android.com/studio/releases/platform-tools" -ForegroundColor Yellow
        exit 1
    }
}

Write-Host "OK ADB trouve: $adbPath" -ForegroundColor Green
Write-Host ""

# 2. Verifier qu'un appareil est connecte
Write-Host "[2/5] Verification des appareils connectes..." -ForegroundColor Yellow
$adbOutput = & adb devices
$deviceList = @()
foreach ($line in $adbOutput) {
    if ($line -match "^\s*([^\s]+)\s+device\s*$") {
        $deviceList += $matches[1]
    }
}

if ($deviceList.Count -eq 0) {
    Write-Host "ERREUR: Aucun appareil Android connecte!" -ForegroundColor Red
    Write-Host "Assurez-vous que:" -ForegroundColor Yellow
    Write-Host "  - Le telephone est connecte en USB" -ForegroundColor Yellow
    Write-Host "  - Le debogage USB est active sur le telephone" -ForegroundColor Yellow
    Write-Host "  - Vous avez autorise l'ordinateur sur le telephone" -ForegroundColor Yellow
    exit 1
}

Write-Host "OK $($deviceList.Count) appareil(s) connecte(s)" -ForegroundColor Green
$firstDevice = $deviceList[0]
Write-Host "  Utilisation du premier appareil: $firstDevice" -ForegroundColor Cyan
Write-Host ""

# 3. Trouver le fichier APK
Write-Host "[3/5] Recherche du fichier APK..." -ForegroundColor Yellow
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

$possiblePaths = @(
    (Join-Path $scriptDir "Godot\MyGodotGame.apk"),
    (Join-Path $scriptDir "..\Godot\untitled-platform-game\Release\MyGodotGame.apk"),
    "C:\Users\drack\Documents\prog\Developpement-Application--Maui-..-\PlatformGame\Godot\MyGodotGame.apk",
    "Godot\MyGodotGame.apk"
)

if ([string]::IsNullOrEmpty($ApkPath)) {
    foreach ($path in $possiblePaths) {
        if (Test-Path $path) {
            $ApkPath = Resolve-Path $path
            break
        }
    }
}

if (-not $ApkPath -or -not (Test-Path $ApkPath)) {
    Write-Host "ERREUR: Fichier APK non trouve!" -ForegroundColor Red
    Write-Host "Chemins recherches:" -ForegroundColor Yellow
    foreach ($path in $possiblePaths) {
        Write-Host "  - $path" -ForegroundColor Yellow
    }
    Write-Host ""
    Write-Host "Specifiez le chemin avec: .\deploy_godot_android.ps1 -ApkPath 'C:\chemin\vers\MyGodotGame.apk'" -ForegroundColor Yellow
    exit 1
}

$ApkPath = Resolve-Path $ApkPath
Write-Host "OK APK trouve: $ApkPath" -ForegroundColor Green
Write-Host ""

# 4. Desinstaller l'ancienne version si elle existe
Write-Host "[4/5] Verification de l'installation existante..." -ForegroundColor Yellow
$packageList = & adb -s $firstDevice shell pm list packages
$isInstalled = $packageList | Select-String $PackageName

if ($isInstalled) {
    Write-Host "  Ancienne version detectee, desinstallation..." -ForegroundColor Yellow
    $uninstallResult = & adb -s $firstDevice uninstall $PackageName 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "OK Ancienne version desinstallee" -ForegroundColor Green
    } else {
        Write-Host "ATTENTION Impossible de desinstaller (peut-etre que l'app n'existe pas): $uninstallResult" -ForegroundColor Yellow
    }
} else {
    Write-Host "  Aucune version precedente trouvee" -ForegroundColor Cyan
}
Write-Host ""

# 5. Installer la nouvelle version
Write-Host "[5/5] Installation de la nouvelle version..." -ForegroundColor Yellow
Write-Host "  Fichier: $ApkPath" -ForegroundColor Cyan
Write-Host "  Appareil: $firstDevice" -ForegroundColor Cyan
Write-Host "  Package: $PackageName" -ForegroundColor Cyan
Write-Host ""

$installResult = & adb -s $firstDevice install -r "$ApkPath" 2>&1
$installOutput = $installResult | Out-String

if ($LASTEXITCODE -eq 0 -or $installOutput -match "Success") {
    Write-Host "OK Installation reussie!" -ForegroundColor Green
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "  Deploiement termine avec succes!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "L'application Godot est maintenant installee sur votre telephone." -ForegroundColor Green
    Write-Host "Vous pouvez la lancer depuis l'app MAUI." -ForegroundColor Green
} else {
    Write-Host "ERREUR lors de l'installation!" -ForegroundColor Red
    Write-Host "Sortie:" -ForegroundColor Yellow
    Write-Host $installOutput -ForegroundColor Red
    Write-Host ""
    Write-Host "Verifiez que:" -ForegroundColor Yellow
    Write-Host "  - Le debogage USB est active" -ForegroundColor Yellow
    Write-Host "  - L'APK n'est pas corrompu" -ForegroundColor Yellow
    Write-Host "  - Il y a assez d'espace sur le telephone" -ForegroundColor Yellow
    exit 1
}
