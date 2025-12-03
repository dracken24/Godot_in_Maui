# Script pour copier le fichier de configuration vers le répertoire de Godot
# À exécuter après avoir lancé l'app MAUI

Write-Host "=== Copie du fichier de configuration vers Godot ===" -ForegroundColor Yellow
Write-Host ""

# Vérifier qu'ADB est disponible
$adbCheck = adb version 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERREUR: ADB n'est pas disponible" -ForegroundColor Red
    exit 1
}

# Chercher le fichier de configuration créé par l'app MAUI
Write-Host "Recherche du fichier de configuration..." -ForegroundColor Cyan
$configFiles = adb shell "find /storage/emulated/0 -name 'platformgame_config.json' 2>/dev/null" | Where-Object { $_ -match "platformgame_config.json" }

if (-not $configFiles) {
    Write-Host "ERREUR: Aucun fichier de configuration trouvé" -ForegroundColor Red
    Write-Host "Lancez d'abord l'app MAUI et cliquez sur 'Lancer le jeu'" -ForegroundColor Yellow
    exit 1
}

$sourceFile = ($configFiles -split "`n")[0].Trim()
Write-Host "Fichier source trouvé: $sourceFile" -ForegroundColor Green

# Lire le contenu
Write-Host "Lecture du contenu..." -ForegroundColor Cyan
$content = adb shell "cat '$sourceFile' 2>&1"
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERREUR: Impossible de lire le fichier" -ForegroundColor Red
    exit 1
}

Write-Host "Contenu: $content" -ForegroundColor Gray
Write-Host ""

# Copier vers le répertoire de Godot
Write-Host "Copie vers le répertoire de Godot..." -ForegroundColor Cyan
$targetPath = "/data/data/com.company.mygodotgame/files/platformgame_config.json"

# Échapper le contenu JSON pour la commande shell
$escapedContent = $content -replace "'", "'\''"

$result = adb shell "run-as com.company.mygodotgame sh -c 'echo ''$escapedContent'' > $targetPath && chmod 644 $targetPath && echo OK'"

if ($result -match "OK") {
    Write-Host "✓ Fichier copié avec succès vers Godot" -ForegroundColor Green
    Write-Host "  Chemin: $targetPath" -ForegroundColor Gray
    
    # Vérifier
    Write-Host "`nVérification..." -ForegroundColor Cyan
    $verify = adb shell "run-as com.company.mygodotgame cat $targetPath 2>&1"
    if ($verify -match "email") {
        Write-Host "✓ Fichier vérifié avec succès" -ForegroundColor Green
        Write-Host "  Contenu: $verify" -ForegroundColor Gray
    } else {
        Write-Host "⚠ ATTENTION: Le fichier pourrait être vide ou corrompu" -ForegroundColor Yellow
    }
} else {
    Write-Host "✗ ERREUR: Impossible de copier le fichier" -ForegroundColor Red
    Write-Host "  Résultat: $result" -ForegroundColor Gray
    exit 1
}

Write-Host "`n=== Terminé ===" -ForegroundColor Green
Write-Host "Relancez Godot et testez à nouveau" -ForegroundColor Yellow

