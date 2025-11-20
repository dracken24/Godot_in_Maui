# Script pour activer le mode debuggable dans un APK Android
# Usage: .\enable_debuggable_apk.ps1 [chemin_vers_apk]

param(
    [string]$ApkPath = ".\Godot\MyGodotGame.apk"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Activation du mode debuggable" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verifier que l'APK existe
if (-not (Test-Path $ApkPath)) {
    Write-Host "ERREUR: APK non trouve: $ApkPath" -ForegroundColor Red
    exit 1
}

Write-Host "[1] Verification d'apktool..." -ForegroundColor Yellow
$apktool = Get-Command apktool -ErrorAction SilentlyContinue
if (-not $apktool) {
    Write-Host "ATTENTION: apktool non trouve!" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Pour activer le mode debuggable, vous avez deux options:" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Option 1: Dans Godot, exporter avec un template de debug" -ForegroundColor Yellow
    Write-Host "  - Projet -> Exporter -> Android -> Options" -ForegroundColor Yellow
    Write-Host "  - Custom Template -> Debug: laisser vide (utilise le template par defaut)" -ForegroundColor Yellow
    Write-Host "  - Ou cocher 'Debuggable' dans les options si disponible" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Option 2: Installer apktool et utiliser ce script" -ForegroundColor Yellow
    Write-Host "  - Telecharger apktool depuis: https://ibotpeaches.github.io/Apktool/" -ForegroundColor Yellow
    Write-Host "  - Ajouter au PATH" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Option 3: Modifier directement dans Godot" -ForegroundColor Yellow
    Write-Host "  - Dans export_presets.cfg, ajouter: application/debuggable=true" -ForegroundColor Yellow
    Write-Host "  - (Deja fait dans votre projet)" -ForegroundColor Green
    Write-Host ""
    exit 0
}

Write-Host "OK apktool trouve" -ForegroundColor Green
Write-Host ""

$workDir = ".\temp_apk_debug"
$apkName = [System.IO.Path]::GetFileNameWithoutExtension($ApkPath)

Write-Host "[2] Decompilation de l'APK..." -ForegroundColor Yellow
if (Test-Path $workDir) {
    Remove-Item -Recurse -Force $workDir
}
apktool d $ApkPath -o $workDir -f
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERREUR: Echec de la decompilation" -ForegroundColor Red
    exit 1
}
Write-Host "OK" -ForegroundColor Green
Write-Host ""

Write-Host "[3] Modification du AndroidManifest.xml..." -ForegroundColor Yellow
$manifestPath = Join-Path $workDir "AndroidManifest.xml"
if (Test-Path $manifestPath) {
    $content = Get-Content $manifestPath -Raw
    # Ajouter android:debuggable="true" dans la balise <application>
    if ($content -notmatch 'android:debuggable="true"') {
        $content = $content -replace '(<application[^>]*)>', '$1 android:debuggable="true">'
        Set-Content -Path $manifestPath -Value $content -NoNewline
        Write-Host "OK Mode debuggable active" -ForegroundColor Green
    } else {
        Write-Host "OK Mode debuggable deja active" -ForegroundColor Green
    }
} else {
    Write-Host "ERREUR: AndroidManifest.xml non trouve" -ForegroundColor Red
    exit 1
}
Write-Host ""

Write-Host "[4] Recompilation de l'APK..." -ForegroundColor Yellow
$outputApk = ".\Godot\MyGodotGame_debuggable.apk"
apktool b $workDir -o $outputApk
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERREUR: Echec de la recompilation" -ForegroundColor Red
    exit 1
}
Write-Host "OK APK recompile: $outputApk" -ForegroundColor Green
Write-Host ""

Write-Host "[5] Signature de l'APK..." -ForegroundColor Yellow
Write-Host "ATTENTION: L'APK doit etre signe pour etre installe." -ForegroundColor Yellow
Write-Host "Utilisez: .\deploy_godot_android.ps1" -ForegroundColor Cyan
Write-Host ""

Write-Host "[6] Nettoyage..." -ForegroundColor Yellow
Remove-Item -Recurse -Force $workDir
Write-Host "OK" -ForegroundColor Green
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Termine!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "L'APK avec mode debuggable est dans: $outputApk" -ForegroundColor Green
Write-Host "Vous pouvez maintenant le deployer avec: .\deploy_godot_android.ps1" -ForegroundColor Cyan

