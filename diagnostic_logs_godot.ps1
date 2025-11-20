# Script de diagnostic pour les logs Godot
# Usage: .\diagnostic_logs_godot.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Diagnostic des logs Godot" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verifier ADB
Write-Host "[1] Verification d'ADB..." -ForegroundColor Yellow
$adbPath = Get-Command adb -ErrorAction SilentlyContinue
if (-not $adbPath) {
    Write-Host "ERREUR: ADB non trouve!" -ForegroundColor Red
    exit 1
}
Write-Host "OK ADB trouve" -ForegroundColor Green
Write-Host ""

# Verifier qu'un appareil est connecte
Write-Host "[2] Verification de l'appareil..." -ForegroundColor Yellow
$devices = & adb devices | Select-Object -Skip 1 | Where-Object { $_ -match "device$" }
if (-not $devices) {
    Write-Host "ERREUR: Aucun appareil Android connecte!" -ForegroundColor Red
    exit 1
}
Write-Host "OK Appareil connecte" -ForegroundColor Green
Write-Host ""

# Verifier si le processus Godot est en cours d'execution
Write-Host "[3] Verification du processus Godot..." -ForegroundColor Yellow
$packageName = "com.company.mygodotgame"
$pidOutput = adb shell "pidof $packageName" 2>$null
if ($pidOutput -and $pidOutput -match '^\d+$') {
    $processId = $pidOutput.Trim()
    Write-Host "OK Processus Godot en cours d'execution (PID: $processId)" -ForegroundColor Green
} else {
    Write-Host "ATTENTION: Processus Godot non trouve" -ForegroundColor Yellow
    Write-Host "  Le jeu n'est peut-etre pas lance. Lancez-le depuis l'app MAUI." -ForegroundColor Yellow
}
Write-Host ""

# Chercher les fichiers de log dans tous les chemins possibles
Write-Host "[4] Recherche des fichiers de log..." -ForegroundColor Yellow

# Obtenir le user data dir de Godot
$userDataDir = adb shell "run-as com.company.mygodotgame sh -c 'echo \$ANDROID_DATA' 2>/dev/null"
if (-not $userDataDir -or $userDataDir -match "error") {
    $userDataDir = "/data/data/com.company.mygodotgame/files"
}

$logPaths = @(
    "$userDataDir/godot_logs.txt",  # User data dir (garanti de fonctionner)
    "/sdcard/Download/godot_logs.txt",
    "/storage/emulated/0/Download/godot_logs.txt",
    "/data/data/com.company.mygodotgame/files/godot_logs.txt"
)

$foundFiles = @()
foreach ($logPath in $logPaths) {
    $exists = adb shell "test -f `"$logPath`" && echo 'EXISTS' || echo 'NOT_EXISTS'"
    if ($exists -match "EXISTS") {
        $size = adb shell "stat -c%s `"$logPath`" 2>/dev/null || echo '0'"
        $size = $size.Trim()
        $foundFiles += @{
            Path = $logPath
            Size = $size
        }
        Write-Host "  OK Trouve: $logPath (taille: $size octets)" -ForegroundColor Green
    } else {
        Write-Host "  - Non trouve: $logPath" -ForegroundColor Gray
    }
}

if ($foundFiles.Count -eq 0) {
    Write-Host ""
    Write-Host "ATTENTION: Aucun fichier de log trouve!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Causes possibles:" -ForegroundColor Yellow
    Write-Host "  1. L'APK n'a pas ete reexporte avec les nouvelles modifications" -ForegroundColor Yellow
    Write-Host "  2. Le jeu n'a pas encore ete lance depuis l'app MAUI" -ForegroundColor Yellow
    Write-Host "  3. Les permissions Android empechent l'ecriture dans /sdcard/Download/" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Actions recommandees:" -ForegroundColor Cyan
    Write-Host "  1. Reexportez l'APK Godot avec les nouvelles modifications" -ForegroundColor Cyan
    Write-Host "  2. Reinstallez l'APK: .\deploy_godot_android.ps1" -ForegroundColor Cyan
    Write-Host "  3. Lancez le jeu depuis l'app MAUI" -ForegroundColor Cyan
    Write-Host "  4. Relancez ce script de diagnostic" -ForegroundColor Cyan
} else {
    Write-Host ""
    Write-Host "[5] Affichage du contenu des fichiers de log..." -ForegroundColor Yellow
    Write-Host ""
    foreach ($file in $foundFiles) {
        Write-Host "--- Contenu de $($file.Path) ---" -ForegroundColor Cyan
        $content = adb shell "cat `"$($file.Path)`""
        if ($content -and $content.Trim().Length -gt 0) {
            Write-Host $content
        } else {
            Write-Host "(fichier vide)" -ForegroundColor Gray
        }
        Write-Host ""
    }
}

Write-Host "[6] Verification des permissions..." -ForegroundColor Yellow
$testPath = "/sdcard/Download"
$canWrite = adb shell "test -w `"$testPath`" && echo 'WRITABLE' || echo 'NOT_WRITABLE'"
if ($canWrite -match "WRITABLE") {
    Write-Host "OK Le dossier $testPath est accessible en ecriture" -ForegroundColor Green
} else {
    Write-Host "ATTENTION: Le dossier $testPath n'est pas accessible en ecriture" -ForegroundColor Yellow
    Write-Host "  Cela peut empecher l'ecriture des logs" -ForegroundColor Yellow
}
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Diagnostic termine" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

