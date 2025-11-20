using System.Net.Http.Json;
using System.Diagnostics;

namespace PlatformGame.Services;

public class GameApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl;

    public GameApiService()
    {
        // Configurer HttpClient avec gestion des certificats pour Android
        var handler = new HttpClientHandler();
        
#if ANDROID
        // Pour Android, accepter tous les certificats en développement (non recommandé pour la production)
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif

        _httpClient = new HttpClient(handler);
        
        // Lire depuis les variables d'environnement ou utiliser la valeur par défaut selon la plateforme
        var envUrl = Environment.GetEnvironmentVariable("API_BASE_URL");
        
        if (!string.IsNullOrEmpty(envUrl))
        {
            _apiBaseUrl = envUrl;
        }
        else
        {
            // Valeurs par défaut selon la plateforme
#if ANDROID
            // Par défaut, utiliser l'IP de la machine (10.0.0.49)
            // Si vous utilisez un émulateur et que 10.0.2.2 ne fonctionne pas,
            // remplacez cette valeur par l'IP de votre machine (trouvez-la avec ipconfig)
            _apiBaseUrl = "http://10.0.0.49:5000";
#elif IOS || MACCATALYST
            _apiBaseUrl = "http://localhost:5000";
#else
            // Windows, Linux, etc.
            _apiBaseUrl = "https://localhost:5001";
#endif
        }
        
        Debug.WriteLine($"GameApiService: URL de l'API configurée: {_apiBaseUrl}");
        
        _httpClient.BaseAddress = new Uri(_apiBaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        
        // Ajouter des en-têtes pour le débogage
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "PlatformGame-MAUI/1.0");
    }

    /// <summary>
    /// Récupère tous les résultats de jeu
    /// </summary>
    public async Task<List<GameResultResponse>> GetAllResultsAsync()
    {
        try
        {
            Debug.WriteLine($"GameApiService: Récupération de tous les résultats depuis {_apiBaseUrl}/api/gameresults/results");
            var response = await _httpClient.GetFromJsonAsync<List<GameResultResponse>>("/api/gameresults/results");
            return response ?? new List<GameResultResponse>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"GameApiService: Erreur lors de la récupération des résultats: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Récupère le meilleur temps (record)
    /// </summary>
    public async Task<GameResultResponse?> GetBestResultAsync()
    {
        try
        {
            Debug.WriteLine($"GameApiService: Récupération du meilleur résultat depuis {_apiBaseUrl}/api/gameresults/results/best");
            var response = await _httpClient.GetAsync("/api/gameresults/results/best");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResultResponse>();
                return result;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Pas de résultats dans la base de données, c'est normal
                Debug.WriteLine("GameApiService: Aucun résultat trouvé dans la base de données");
                return null;
            }
            else
            {
                Debug.WriteLine($"GameApiService: Erreur HTTP {response.StatusCode} lors de la récupération du meilleur résultat");
                return null;
            }
        }
        catch (HttpRequestException httpEx)
        {
            Debug.WriteLine($"GameApiService: Erreur HTTP lors de la récupération du meilleur résultat: {httpEx.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"GameApiService: Erreur lors de la récupération du meilleur résultat: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Récupère les N meilleurs temps (ex: top 10)
    /// </summary>
    public async Task<List<GameResultResponse>> GetTopResultsAsync(int count)
    {
        try
        {
            Debug.WriteLine($"GameApiService: Récupération des top {count} résultats depuis {_apiBaseUrl}/api/gameresults/results/top/{count}");
            var response = await _httpClient.GetAsync($"/api/gameresults/results/top/{count}");
            
            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadFromJsonAsync<List<GameResultResponse>>();
                return results ?? new List<GameResultResponse>();
            }
            else
            {
                Debug.WriteLine($"GameApiService: Erreur HTTP {response.StatusCode} lors de la récupération des top résultats");
                return new List<GameResultResponse>();
            }
        }
        catch (HttpRequestException httpEx)
        {
            Debug.WriteLine($"GameApiService: Erreur HTTP lors de la récupération des top résultats: {httpEx.Message}");
            return new List<GameResultResponse>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"GameApiService: Erreur lors de la récupération des top résultats: {ex.Message}");
            return new List<GameResultResponse>();
        }
    }

    /// <summary>
    /// Enregistre un nouveau temps de completion (appelé par le jeu Godot)
    /// </summary>
    public async Task<GameResultResponse?> CreateResultAsync(CreateGameResultRequest request)
    {
        try
        {
            Debug.WriteLine($"GameApiService: Envoi d'un nouveau résultat: {request.CompletionTimeMs}ms");
            var response = await _httpClient.PostAsJsonAsync("/api/gameresults/results", request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<GameResultResponse>();
            return result;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"GameApiService: Erreur lors de la création du résultat: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Récupère les statistiques (moyenne, nombre de tentatives, meilleur temps, etc.)
    /// </summary>
    public async Task<GameStatsResponse?> GetStatsAsync()
    {
        try
        {
            Debug.WriteLine($"GameApiService: Récupération des statistiques depuis {_apiBaseUrl}/api/gameresults/stats");
            
            // Test de connexion basique d'abord
            try
            {
                var testResponse = await _httpClient.GetAsync("/api/gameresults/test");
                Debug.WriteLine($"GameApiService: Test de connexion - Status: {testResponse.StatusCode}");
                if (!testResponse.IsSuccessStatusCode)
                {
                    var errorContent = await testResponse.Content.ReadAsStringAsync();
                    Debug.WriteLine($"GameApiService: Erreur de test - {errorContent}");
                }
            }
            catch (Exception testEx)
            {
                Debug.WriteLine($"GameApiService: Erreur lors du test de connexion: {testEx.GetType().Name} - {testEx.Message}");
                Debug.WriteLine($"GameApiService: Stack trace: {testEx.StackTrace}");
            }
            
            var response = await _httpClient.GetFromJsonAsync<GameStatsResponse>("/api/gameresults/stats");
            return response;
        }
        catch (HttpRequestException httpEx)
        {
            Debug.WriteLine($"GameApiService: Erreur HTTP lors de la récupération des statistiques: {httpEx.Message}");
            Debug.WriteLine($"GameApiService: Inner exception: {httpEx.InnerException?.Message}");
            throw;
        }
        catch (TaskCanceledException timeoutEx)
        {
            Debug.WriteLine($"GameApiService: Timeout lors de la récupération des statistiques: {timeoutEx.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"GameApiService: Erreur lors de la récupération des statistiques: {ex.GetType().Name} - {ex.Message}");
            Debug.WriteLine($"GameApiService: Stack trace: {ex.StackTrace}");
            throw;
        }
    }
}

// Modèles de données pour MAUI (correspondent aux DTOs de l'API)
public class GameResultResponse
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public long CompletionTimeMs { get; set; }
    public string CompletionTimeFormatted { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? Platform { get; set; }
}

public class CreateGameResultRequest
{
    public long CompletionTimeMs { get; set; }
    public string? Email { get; set; }
    public string? Platform { get; set; }
}

public class GameStatsResponse
{
    public long BestTimeMs { get; set; }
    public string BestTimeFormatted { get; set; } = string.Empty;
    public long AverageTimeMs { get; set; }
    public string AverageTimeFormatted { get; set; } = string.Empty;
    public int TotalAttempts { get; set; }
    public GameResultResponse? BestResult { get; set; }
}

