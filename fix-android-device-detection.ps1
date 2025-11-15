# Script pour diagnostiquer et résoudre les problèmes de détection d'appareil Android
# Usage: .\fix-android-device-detection.ps1

Write-Host "=== Diagnostic de détection d'appareil Android ===" -ForegroundColor Cyan

# Vérifier que ADB est disponible
$adbPath = Get-Command adb -ErrorAction SilentlyContinue
if (-not $adbPath) {
    Write-Host "ERREUR: ADB n'est pas trouvé dans le PATH" -ForegroundColor Red
    Write-Host "Assurez-vous que le SDK Android est installé et que ADB est dans votre PATH" -ForegroundColor Yellow
    exit 1
}

Write-Host "`n1. Vérification des appareils connectés..." -ForegroundColor Yellow
$devices = adb devices
Write-Host $devices

# Analyser les résultats
$deviceLines = $devices | Select-String -Pattern "^\w+\s+\w+" | ForEach-Object { $_.ToString().Trim() }
$unauthorizedDevices = $deviceLines | Where-Object { $_ -match "unauthorized" }
$authorizedDevices = $deviceLines | Where-Object { $_ -match "device$" }
$offlineDevices = $deviceLines | Where-Object { $_ -match "offline" }

if ($unauthorizedDevices) {
    Write-Host "`n⚠️  PROBLÈME DÉTECTÉ: Appareil(s) non autorisé(s)" -ForegroundColor Red
    Write-Host "`nSOLUTION:" -ForegroundColor Yellow
    Write-Host "1. Regardez votre téléphone Android" -ForegroundColor White
    Write-Host "2. Une fenêtre popup devrait apparaître avec 'Autoriser le débogage USB ?'" -ForegroundColor White
    Write-Host "3. Cochez 'Toujours autoriser depuis cet ordinateur'" -ForegroundColor White
    Write-Host "4. Appuyez sur 'Autoriser' ou 'OK'" -ForegroundColor White
    Write-Host "`nSi la fenêtre n'apparaît pas:" -ForegroundColor Yellow
    Write-Host "  • Débranchez et rebranchez le câble USB" -ForegroundColor White
    Write-Host "  • Sur votre téléphone: Paramètres → Système → Options développeur" -ForegroundColor White
    Write-Host "  • Désactivez puis réactivez 'Débogage USB'" -ForegroundColor White
    
    Write-Host "`nAppuyez sur Entrée après avoir autorisé le débogage sur votre téléphone..." -ForegroundColor Cyan
    Read-Host
    
    Write-Host "`nVérification après autorisation..." -ForegroundColor Yellow
    adb kill-server
    Start-Sleep -Seconds 2
    adb start-server
    Start-Sleep -Seconds 2
    $devicesAfter = adb devices
    Write-Host $devicesAfter
    
    $deviceLinesAfter = $devicesAfter | Select-String -Pattern "^\w+\s+\w+" | ForEach-Object { $_.ToString().Trim() }
    $authorizedAfter = $deviceLinesAfter | Where-Object { $_ -match "device$" }
    
    if ($authorizedAfter) {
        Write-Host "`n✅ SUCCÈS: Appareil autorisé!" -ForegroundColor Green
        Write-Host "Visual Studio devrait maintenant détecter votre appareil." -ForegroundColor Green
    } else {
        Write-Host "`n❌ L'appareil n'est toujours pas autorisé." -ForegroundColor Red
        Write-Host "Essayez de révoquer les autorisations USB sur votre téléphone:" -ForegroundColor Yellow
        Write-Host "  Paramètres → Système → Options développeur → Révoquer les autorisations de débogage USB" -ForegroundColor White
    }
} elseif ($authorizedDevices) {
    Write-Host "`n✅ SUCCÈS: Appareil(s) autorisé(s) et détecté(s)!" -ForegroundColor Green
    foreach ($device in $authorizedDevices) {
        $deviceId = ($device -split '\s+')[0]
        Write-Host "  • $deviceId" -ForegroundColor Green
    }
    Write-Host "`nSi Visual Studio ne détecte toujours pas l'appareil:" -ForegroundColor Yellow
    Write-Host "  1. Redémarrez Visual Studio" -ForegroundColor White
    Write-Host "  2. Vérifiez: Outils → Options → Xamarin → Paramètres Android" -ForegroundColor White
    Write-Host "  3. Cliquez sur 'Actualiser' dans la section Android SDK" -ForegroundColor White
} elseif ($offlineDevices) {
    Write-Host "`n⚠️  PROBLÈME: Appareil(s) hors ligne" -ForegroundColor Yellow
    Write-Host "SOLUTION:" -ForegroundColor Yellow
    Write-Host "  • Débranchez et rebranchez le câble USB" -ForegroundColor White
    Write-Host "  • Assurez-vous que le mode de transfert de fichiers (MTP) est activé" -ForegroundColor White
    Write-Host "  • Essayez un autre câble USB ou un autre port USB" -ForegroundColor White
} else {
    Write-Host "`n⚠️  Aucun appareil détecté" -ForegroundColor Yellow
    Write-Host "Vérifications:" -ForegroundColor Yellow
    Write-Host "  • Le câble USB est bien branché" -ForegroundColor White
    Write-Host "  • Le débogage USB est activé sur votre téléphone" -ForegroundColor White
    Write-Host "  • Le mode développeur est activé" -ForegroundColor White
    Write-Host "  • Le mode de transfert de fichiers (MTP) est sélectionné" -ForegroundColor White
    Write-Host "  • Les pilotes USB sont installés (vérifiez dans le Gestionnaire de périphériques)" -ForegroundColor White
}

# Vérifier les informations détaillées
if ($authorizedDevices) {
    Write-Host "`n2. Informations détaillées des appareils..." -ForegroundColor Yellow
    foreach ($device in $authorizedDevices) {
        $deviceId = ($device -split '\s+')[0]
        Write-Host "`nAppareil: $deviceId" -ForegroundColor Cyan
        $deviceInfo = adb -s $deviceId shell getprop ro.product.model 2>&1
        if ($deviceInfo -and -not ($deviceInfo -match "error")) {
            Write-Host "  Modèle: $deviceInfo" -ForegroundColor Green
        }
        $androidVersion = adb -s $deviceId shell getprop ro.build.version.release 2>&1
        if ($androidVersion -and -not ($androidVersion -match "error")) {
            Write-Host "  Android: $androidVersion" -ForegroundColor Green
        }
    }
}

Write-Host "`n=== Fin du diagnostic ===" -ForegroundColor Cyan


