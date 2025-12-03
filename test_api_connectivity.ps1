# Script pour tester la connectivité de l'API depuis l'appareil Android
Write-Host "=== Test de connectivité API ===" -ForegroundColor Yellow
Write-Host ""

# 1. Vérifier que l'API est en cours d'exécution
Write-Host "[1] Vérification de l'API locale..." -ForegroundColor Cyan
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/gameresults/test" -TimeoutSec 5 -UseBasicParsing
    Write-Host "OK: API répond en localhost (Status: $($response.StatusCode))" -ForegroundColor Green
    Write-Host "   Réponse: $($response.Content)" -ForegroundColor Gray
} catch {
    Write-Host "ERREUR: API ne répond pas en localhost" -ForegroundColor Red
    Write-Host "   Message: $($_.Exception.Message)" -ForegroundColor Gray
    exit 1
}

Write-Host ""

# 2. Vérifier l'IP de la machine
Write-Host "[2] Vérification de l'IP de la machine..." -ForegroundColor Cyan
$ipAddresses = Get-NetIPAddress -AddressFamily IPv4 | Where-Object { $_.IPAddress -notlike "127.*" -and $_.IPAddress -notlike "169.254.*" } | Select-Object -ExpandProperty IPAddress
Write-Host "IPs disponibles:" -ForegroundColor Yellow
foreach ($ip in $ipAddresses) {
    Write-Host "  - $ip" -ForegroundColor White
}

Write-Host ""

# 3. Vérifier que le port 5000 est accessible depuis l'extérieur
Write-Host "[3] Vérification du port 5000..." -ForegroundColor Cyan
$listening = netstat -ano | Select-String "LISTENING" | Select-String ":5000"
if ($listening) {
    Write-Host "OK: Port 5000 en écoute" -ForegroundColor Green
    Write-Host "   $listening" -ForegroundColor Gray
} else {
    Write-Host "ERREUR: Port 5000 n'est pas en écoute" -ForegroundColor Red
}

Write-Host ""

# 4. Tester depuis l'appareil Android (si connecté)
Write-Host "[4] Test depuis l'appareil Android..." -ForegroundColor Cyan
$devices = adb devices | Select-String "device$"
if ($devices) {
    Write-Host "Appareil Android détecté" -ForegroundColor Green
    
    # Test de ping
    Write-Host "   Test de ping vers 10.0.0.49..." -ForegroundColor Yellow
    $pingResult = adb shell "ping -c 2 10.0.0.49 2>&1"
    if ($pingResult -match "0% packet loss" -or $pingResult -match "1 received") {
        Write-Host "   OK: Ping réussi" -ForegroundColor Green
    } else {
        Write-Host "   ERREUR: Ping échoué" -ForegroundColor Red
        Write-Host "   Résultat: $pingResult" -ForegroundColor Gray
    }
    
    # Test de connexion HTTP
    Write-Host "   Test de connexion HTTP vers http://10.0.0.49:5000..." -ForegroundColor Yellow
    $httpTest = adb shell "timeout 5 sh -c 'exec 3<>/dev/tcp/10.0.0.49/5000 && echo -e \"GET /api/gameresults/test HTTP/1.1\r\nHost: 10.0.0.49:5000\r\n\r\n\" >&3 && cat <&3' 2>&1"
    if ($httpTest -match "200" -or $httpTest -match "OK") {
        Write-Host "   OK: Connexion HTTP réussie" -ForegroundColor Green
    } else {
        Write-Host "   ERREUR: Connexion HTTP échouée" -ForegroundColor Red
        Write-Host "   Résultat: $httpTest" -ForegroundColor Gray
        Write-Host ""
        Write-Host "   SOLUTIONS:" -ForegroundColor Yellow
        Write-Host "   1. Vérifiez que Avast autorise les connexions entrantes sur le port 5000" -ForegroundColor Cyan
        Write-Host "   2. Vérifiez que le firewall Windows autorise le port 5000" -ForegroundColor Cyan
        Write-Host "   3. Vérifiez que l'IP 10.0.0.49 est correcte (ipconfig)" -ForegroundColor Cyan
    }
} else {
    Write-Host "Aucun appareil Android connecté" -ForegroundColor Yellow
    Write-Host "   Connectez votre appareil et activez le débogage USB" -ForegroundColor Gray
}

Write-Host ""
Write-Host "=== Fin du test ===" -ForegroundColor Yellow

