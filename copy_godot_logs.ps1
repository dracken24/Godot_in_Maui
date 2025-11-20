# Script pour copier les logs Godot depuis user:// vers un emplacement accessible
# Usage: .\copy_godot_logs.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Copie des logs Godot" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$packageName = "com.company.mygodotgame"
$logFile = "files/godot_logs.txt"
$outputFile = ".\godot_logs_copie.txt"

Write-Host "[1] Verification du processus Godot..." -ForegroundColor Yellow
$processId = adb shell "pidof $packageName" 2>$null
if (-not $processId -or $processId -notmatch '^\d+$') {
    Write-Host "ATTENTION: Le jeu n'est pas lance." -ForegroundColor Yellow
    Write-Host "  Lancez le jeu depuis l'app MAUI d'abord." -ForegroundColor Yellow
    Write-Host ""
}

Write-Host "[2] Tentative de copie du fichier de log..." -ForegroundColor Yellow
Write-Host "  (Cela ne fonctionnera que si l'APK est en mode debuggable)" -ForegroundColor Gray
Write-Host ""

# Essayer de copier avec run-as
$content = adb shell "run-as $packageName cat $logFile 2>&1"
if ($content -match "package not debuggable") {
    Write-Host "ERREUR: L'APK n'est pas en mode debuggable." -ForegroundColor Red
    Write-Host ""
    Write-Host "Solutions:" -ForegroundColor Yellow
    Write-Host "  1. Activer le mode debuggable dans Godot:" -ForegroundColor Cyan
    Write-Host "     - Projet -> Exporter -> Android -> Options" -ForegroundColor Cyan
    Write-Host "     - Custom Template -> Debug: laisser vide" -ForegroundColor Cyan
    Write-Host "     - Reexporter l'APK" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  2. Utiliser les logs dans /sdcard/Download/ (si l'ecriture fonctionne)" -ForegroundColor Cyan
    Write-Host "     .\watch_godot_logs.ps1" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  3. Verifier les logs Android directement:" -ForegroundColor Cyan
    Write-Host "     adb logcat | Select-String -Pattern 'Godot|UserEmailManager'" -ForegroundColor Cyan
} elseif ($content -and $content.Trim().Length -gt 0) {
    Write-Host "OK Fichier de log copie!" -ForegroundColor Green
    Write-Host ""
    Write-Host "[3] Affichage du contenu (formate)..." -ForegroundColor Yellow
    Write-Host ""
    # Formater le contenu pour une meilleure lisibilite
    # Separer les messages qui sont colles ensemble (pattern: ] [)
    $formattedContent = $content -replace '\]\s+\[', "]`r`n["
    # Ajouter des retours a la ligne avant les messages ERROR
    $formattedContent = $formattedContent -replace '\s+ERROR:', "`r`nERROR:"
    # Nettoyer les lignes vides multiples (plus de 2 lignes vides consecutives)
    $formattedContent = $formattedContent -replace '(`r`n\s*){3,}', "`r`n`r`n"
    # Afficher ligne par ligne avec coloration
    $lines = $formattedContent -split "`r`n"
    foreach ($line in $lines) {
        $trimmedLine = $line.Trim()
        if ($trimmedLine.Length -eq 0) {
            # Ne pas afficher les lignes vides consecutives
            continue
        }
        if ($trimmedLine -match "ERROR:|ERREUR|ECHEC") {
            Write-Host $trimmedLine -ForegroundColor Red
        } elseif ($trimmedLine -match "\[OK\]|\[SUCCES\]|succes|trouve et accessible|charge depuis") {
            Write-Host $trimmedLine -ForegroundColor Green
        } elseif ($trimmedLine -match "Verification|Recherche|User data dir") {
            Write-Host $trimmedLine -ForegroundColor Cyan
        } elseif ($trimmedLine -match "^\[20\d{2}-") {
            # Timestamp seul sur une ligne - ne pas l'afficher
            continue
        } else {
            Write-Host $trimmedLine
        }
    }
    Write-Host ""
    Write-Host "[4] Sauvegarde dans $outputFile..." -ForegroundColor Yellow
    $content | Out-File -FilePath $outputFile -Encoding UTF8
    Write-Host "OK Fichier sauvegarde: $outputFile" -ForegroundColor Green
} else {
    Write-Host "ATTENTION: Fichier de log vide ou erreur." -ForegroundColor Yellow
    Write-Host "  Contenu recu: $content" -ForegroundColor Gray
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan

