# Script de vérification de la configuration Android pour Godot
# Usage: .\check-android-setup.ps1

Write-Host "=== Vérification de la configuration Android pour Godot ===" -ForegroundColor Cyan

$allGood = $true

# Vérifier Java/JDK
Write-Host "`n1. Vérification de Java/JDK..." -ForegroundColor Yellow
try {
    $javaVersion = java -version 2>&1 | Select-Object -First 1
    Write-Host "   Java trouvé: $javaVersion" -ForegroundColor Green
    
    # Vérifier la version (doit être 17+)
    $versionMatch = $javaVersion | Select-String -Pattern "(\d+)\.(\d+)" | ForEach-Object { $_.Matches[0].Value }
    Write-Host "   Version: $versionMatch" -ForegroundColor Green
} catch {
    Write-Host "   ERREUR: Java/JDK non trouvé" -ForegroundColor Red
    Write-Host "   Installez JDK 17 ou supérieur depuis: https://adoptium.net/" -ForegroundColor Yellow
    $allGood = $false
}

# Vérifier JAVA_HOME
Write-Host "`n2. Vérification de JAVA_HOME..." -ForegroundColor Yellow
$javaHome = $env:JAVA_HOME
if ($javaHome) {
    Write-Host "   JAVA_HOME: $javaHome" -ForegroundColor Green
} else {
    Write-Host "   ATTENTION: JAVA_HOME n'est pas défini" -ForegroundColor Yellow
    Write-Host "   Ce n'est pas critique mais recommandé" -ForegroundColor Yellow
}

# Vérifier Android SDK
Write-Host "`n3. Vérification de Android SDK..." -ForegroundColor Yellow
$sdkPaths = @(
    "$env:LOCALAPPDATA\Android\Sdk",
    "$env:USERPROFILE\AppData\Local\Android\Sdk",
    "C:\Android\Sdk"
)

$sdkFound = $false
foreach ($path in $sdkPaths) {
    if (Test-Path $path) {
        Write-Host "   SDK trouvé: $path" -ForegroundColor Green
        $sdkFound = $true
        
        # Vérifier les outils nécessaires
        $platformTools = Join-Path $path "platform-tools\adb.exe"
        if (Test-Path $platformTools) {
            Write-Host "   ADB trouvé: $platformTools" -ForegroundColor Green
        } else {
            Write-Host "   ATTENTION: ADB non trouvé dans platform-tools" -ForegroundColor Yellow
        }
        break
    }
}

if (-not $sdkFound) {
    Write-Host "   ERREUR: Android SDK non trouvé" -ForegroundColor Red
    Write-Host "   Installez Android Studio depuis: https://developer.android.com/studio" -ForegroundColor Yellow
    Write-Host "   Ou téléchargez le SDK standalone" -ForegroundColor Yellow
    $allGood = $false
}

# Vérifier ADB dans le PATH
Write-Host "`n4. Vérification de ADB dans le PATH..." -ForegroundColor Yellow
try {
    $adbPath = Get-Command adb -ErrorAction SilentlyContinue
    if ($adbPath) {
        Write-Host "   ADB trouvé: $($adbPath.Source)" -ForegroundColor Green
        
        # Vérifier les appareils
        Write-Host "`n5. Vérification des appareils connectés..." -ForegroundColor Yellow
        $devices = adb devices
        Write-Host $devices
        
        $deviceCount = (adb devices | Select-String -Pattern "device$" | Measure-Object).Count
        if ($deviceCount -gt 0) {
            Write-Host "   $deviceCount appareil(s) connecté(s)" -ForegroundColor Green
        } else {
            Write-Host "   ATTENTION: Aucun appareil connecté" -ForegroundColor Yellow
            Write-Host "   Démarrez un émulateur ou connectez un appareil physique" -ForegroundColor Yellow
        }
    } else {
        Write-Host "   ATTENTION: ADB non trouvé dans le PATH" -ForegroundColor Yellow
        Write-Host "   Ajoutez le SDK platform-tools à votre PATH" -ForegroundColor Yellow
    }
} catch {
    Write-Host "   ERREUR: ADB non disponible" -ForegroundColor Red
    $allGood = $false
}

# Vérifier keytool (pour créer un keystore)
Write-Host "`n6. Vérification de keytool..." -ForegroundColor Yellow
try {
    $keytoolPath = Get-Command keytool -ErrorAction SilentlyContinue
    if ($keytoolPath) {
        Write-Host "   keytool trouvé: $($keytoolPath.Source)" -ForegroundColor Green
    } else {
        Write-Host "   ATTENTION: keytool non trouvé" -ForegroundColor Yellow
        Write-Host "   keytool est inclus avec JDK, vérifiez votre installation Java" -ForegroundColor Yellow
    }
} catch {
    Write-Host "   ATTENTION: keytool non disponible" -ForegroundColor Yellow
}

# Résumé
Write-Host "`n=== Résumé ===" -ForegroundColor Cyan
if ($allGood) {
    Write-Host "Configuration OK! Vous pouvez exporter votre APK depuis Godot." -ForegroundColor Green
} else {
    Write-Host "Certains éléments manquent. Corrigez les erreurs ci-dessus avant d'exporter." -ForegroundColor Red
}

Write-Host "`nPour exporter depuis Godot:" -ForegroundColor Cyan
Write-Host "1. Project → Export" -ForegroundColor White
Write-Host "2. Ajoutez un preset Android si nécessaire" -ForegroundColor White
Write-Host "3. Configurez Package/App Name: com.company.mygodotgame" -ForegroundColor White
Write-Host "4. Export Project → Choisissez un emplacement" -ForegroundColor White

