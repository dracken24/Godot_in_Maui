# Script pour voir les logs de l'API en temps réel
# Surveille les requêtes POST vers /api/gameresults/results

Write-Host "=== Logs API (Requêtes de scores) ===" -ForegroundColor Yellow
Write-Host "Surveille les requêtes POST vers /api/gameresults/results" -ForegroundColor Cyan
Write-Host "Appuyez sur Ctrl+C pour arrêter" -ForegroundColor Gray
Write-Host ""

# Vérifier si l'API est en cours d'exécution
$apiProcess = Get-Process -Name "PlatformGame.Api" -ErrorAction SilentlyContinue
if (-not $apiProcess) {
    Write-Host "ATTENTION: L'API ne semble pas être en cours d'exécution" -ForegroundColor Yellow
    Write-Host "Démarrez l'API avec: cd PlatformGame.Api && dotnet run" -ForegroundColor Cyan
    Write-Host ""
}

# Options
Write-Host "Options:" -ForegroundColor Cyan
Write-Host "1. Voir les logs de l'API (si lancée avec dotnet run)" -ForegroundColor White
Write-Host "2. Tester l'endpoint POST manuellement" -ForegroundColor White
Write-Host "3. Vérifier les dernières requêtes dans la base de données" -ForegroundColor White
Write-Host ""

$choice = Read-Host "Choisissez une option (1-3)"

switch ($choice) {
    "1" {
        Write-Host "Les logs de l'API s'affichent dans la console où vous avez lancé 'dotnet run'" -ForegroundColor Yellow
        Write-Host "Recherchez les lignes avec 'REQUÊTE REÇUE' ou 'Erreur lors de la création'" -ForegroundColor Cyan
    }
    "2" {
        Write-Host "Test de l'endpoint POST..." -ForegroundColor Yellow
        $testData = @{
            email = "test@example.com"
            completionTimeMs = 5000
            platform = "Android"
        } | ConvertTo-Json
        
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:5000/api/gameresults/results" `
                -Method POST `
                -ContentType "application/json" `
                -Body $testData `
                -UseBasicParsing
            
            Write-Host "SUCCÈS: Status $($response.StatusCode)" -ForegroundColor Green
            Write-Host "Réponse: $($response.Content)" -ForegroundColor Gray
        } catch {
            Write-Host "ERREUR: $($_.Exception.Message)" -ForegroundColor Red
            if ($_.Exception.Response) {
                $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
                $responseBody = $reader.ReadToEnd()
                Write-Host "Détails: $responseBody" -ForegroundColor Gray
            }
        }
    }
    "3" {
        Write-Host "Récupération des derniers résultats..." -ForegroundColor Yellow
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:5000/api/gameresults/results" `
                -UseBasicParsing
            
            $results = $response.Content | ConvertFrom-Json
            Write-Host "Nombre de résultats: $($results.Count)" -ForegroundColor Green
            Write-Host ""
            Write-Host "5 derniers résultats:" -ForegroundColor Cyan
            $results | Select-Object -First 5 | Format-Table Email, CompletionTimeFormatted, Platform, CreatedAt
        } catch {
            Write-Host "ERREUR: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
    default {
        Write-Host "Option invalide" -ForegroundColor Red
    }
}

