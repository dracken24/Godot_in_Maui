# Script de test pour copier le fichier de configuration dans le user data dir de Godot
# Usage: .\test_copy_config.ps1 [email]

param(
    [string]$Email = "test@example.com"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Test de copie du fichier de config" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$apiBaseUrl = "http://10.0.0.49:5000"
$godotUserDataDir = "/data/data/com.company.mygodotgame/files"
$targetFile = "$godotUserDataDir/platformgame_config.json"

# Creer le contenu JSON
$config = @{
    email = $Email
    apiBaseUrl = $apiBaseUrl
    timestamp = (Get-Date).ToUniversalTime().ToString("o")
} | ConvertTo-Json -Compress

Write-Host "[1] Contenu JSON a copier:" -ForegroundColor Yellow
Write-Host $config -ForegroundColor Gray
Write-Host ""

# Encoder en base64
$bytes = [System.Text.Encoding]::UTF8.GetBytes($config)
$base64 = [Convert]::ToBase64String($bytes)

Write-Host "[2] Base64 (premiers 50 caracteres):" -ForegroundColor Yellow
Write-Host $base64.Substring(0, [Math]::Min(50, $base64.Length)) -ForegroundColor Gray
Write-Host ""

# Tester la commande ADB
Write-Host "[3] Test de la commande ADB..." -ForegroundColor Yellow
Write-Host "  Commande: adb shell \"run-as com.company.mygodotgame sh -c 'echo $base64 | base64 -d > $targetFile'\"" -ForegroundColor Gray
Write-Host ""

$result = adb shell "run-as com.company.mygodotgame sh -c 'echo $base64 | base64 -d > $targetFile'" 2>&1

if ($LASTEXITCODE -eq 0 -and $result -notmatch "error|Error|ERROR|package not debuggable") {
    Write-Host "OK Commande executee avec succes" -ForegroundColor Green
    Write-Host ""
    
    # Verifier le fichier
    Write-Host "[4] Verification du fichier..." -ForegroundColor Yellow
    $verify = adb shell "run-as com.company.mygodotgame cat $targetFile" 2>&1
    
    if ($verify -and $verify -notmatch "package not debuggable" -and $verify -notmatch "No such file") {
        Write-Host "OK Fichier trouve et contenu:" -ForegroundColor Green
        Write-Host $verify -ForegroundColor Gray
        
        # Verifier que l'email est present
        if ($verify -match $Email) {
            Write-Host ""
            Write-Host "OK Email confirme dans le fichier: $Email" -ForegroundColor Green
        } else {
            Write-Host ""
            Write-Host "ATTENTION: Email non trouve dans le fichier!" -ForegroundColor Yellow
        }
    } else {
        Write-Host "ERREUR: Impossible de lire le fichier" -ForegroundColor Red
        Write-Host "  Reponse: $verify" -ForegroundColor Gray
    }
} else {
    Write-Host "ERREUR: La commande ADB a echoue" -ForegroundColor Red
    Write-Host "  ExitCode: $LASTEXITCODE" -ForegroundColor Gray
    Write-Host "  Reponse: $result" -ForegroundColor Gray
    Write-Host ""
    
    if ($result -match "package not debuggable") {
        Write-Host "SOLUTION: L'APK doit etre en mode debuggable" -ForegroundColor Yellow
        Write-Host "  Dans Godot: Projet -> Exporter -> Android -> Options" -ForegroundColor Cyan
        Write-Host "  Custom Template -> Debug: laisser vide (utilise le template debug par defaut)" -ForegroundColor Cyan
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan

