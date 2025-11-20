using System.Net.Http.Json;
using System.Text.Json;
using System.Diagnostics;

namespace PlatformGame.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl;

    public AuthService()
    {
        var handler = new HttpClientHandler();
        
#if ANDROID
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif

        _httpClient = new HttpClient(handler);
        
        // Logique de base URL identique à GameApiService
        var envUrl = Environment.GetEnvironmentVariable("API_BASE_URL");
        
        if (!string.IsNullOrEmpty(envUrl))
        {
            _apiBaseUrl = envUrl;
        }
        else
        {
#if ANDROID
            // Par défaut, utiliser l'IP de la machine (10.0.0.49)
            // Si vous utilisez un émulateur et que 10.0.2.2 ne fonctionne pas,
            // remplacez cette valeur par l'IP de votre machine (trouvez-la avec ipconfig)
            _apiBaseUrl = "http://10.0.0.49:5000";
#elif IOS || MACCATALYST
            _apiBaseUrl = "http://localhost:5000";
#else
            _apiBaseUrl = "https://localhost:5001";
#endif
        }
        
        Debug.WriteLine($"AuthService: URL de l'API configurée: {_apiBaseUrl}");
        
        _httpClient.BaseAddress = new Uri(_apiBaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<AuthResponse?> LoginAsync(string email, string password)
    {
        try
        {
            Debug.WriteLine($"AuthService: Tentative de connexion pour {email}");
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    await SecureStorage.Default.SetAsync("auth_token", result.Token);
                    await SecureStorage.Default.SetAsync("user_email", email);
                    Debug.WriteLine("AuthService: Connexion réussie");
                }
                return result;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"AuthService: Échec de connexion - Status: {response.StatusCode}, Content: {errorContent}");
                
                // Essayer de parser le message d'erreur de l'API
                string errorMessage = "Échec de la connexion";
                try
                {
                    var errorJson = JsonSerializer.Deserialize<JsonElement>(errorContent);
                    if (errorJson.TryGetProperty("error", out var errorProp))
                    {
                        errorMessage = errorProp.GetString() ?? errorMessage;
                    }
                    else if (errorJson.TryGetProperty("message", out var messageProp))
                    {
                        errorMessage = messageProp.GetString() ?? errorMessage;
                    }
                }
                catch
                {
                    // Si le parsing échoue, utiliser le contenu brut ou un message par défaut
                    if (!string.IsNullOrWhiteSpace(errorContent) && errorContent.Length < 200)
                    {
                        errorMessage = errorContent;
                    }
                }
                
                throw new Exception(errorMessage);
            }
        }
        catch (HttpRequestException httpEx)
        {
            Debug.WriteLine($"AuthService: Erreur HTTP lors de la connexion: {httpEx.Message}");
            Debug.WriteLine($"AuthService: Inner exception: {httpEx.InnerException?.Message}");
            throw;
        }
        catch (TaskCanceledException timeoutEx)
        {
            Debug.WriteLine($"AuthService: Timeout lors de la connexion: {timeoutEx.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AuthService: Erreur lors de la connexion: {ex.GetType().Name} - {ex.Message}");
            Debug.WriteLine($"AuthService: Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<bool> RegisterAsync(string email, string password, string username)
    {
        try
        {
            Debug.WriteLine($"AuthService: Tentative d'inscription pour {email} (username: {username})");
            
            // Test de connexion basique d'abord
            try
            {
                var testResponse = await _httpClient.GetAsync("/api/gameresults/test");
                Debug.WriteLine($"AuthService: Test de connexion - Status: {testResponse.StatusCode}");
                if (!testResponse.IsSuccessStatusCode)
                {
                    var errorContent = await testResponse.Content.ReadAsStringAsync();
                    Debug.WriteLine($"AuthService: Erreur de test - {errorContent}");
                }
            }
            catch (Exception testEx)
            {
                Debug.WriteLine($"AuthService: Erreur lors du test de connexion: {testEx.GetType().Name} - {testEx.Message}");
                Debug.WriteLine($"AuthService: Stack trace: {testEx.StackTrace}");
            }
            
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password, Username = username });
            
            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine("AuthService: Inscription réussie");
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"AuthService: Échec d'inscription - Status: {response.StatusCode}, Content: {errorContent}");
                
                // Essayer de parser le message d'erreur de l'API
                string errorMessage = "L'inscription a échoué";
                try
                {
                    var errorJson = JsonSerializer.Deserialize<JsonElement>(errorContent);
                    if (errorJson.TryGetProperty("error", out var errorProp))
                    {
                        var errorStr = errorProp.GetString();
                        if (!string.IsNullOrEmpty(errorStr))
                        {
                            // Essayer de parser le JSON imbriqué dans la chaîne error
                            try
                            {
                                var nestedError = JsonSerializer.Deserialize<JsonElement>(errorStr);
                                if (nestedError.TryGetProperty("msg", out var msgProp))
                                {
                                    errorMessage = msgProp.GetString() ?? errorMessage;
                                }
                                else if (nestedError.TryGetProperty("message", out var messageProp))
                                {
                                    errorMessage = messageProp.GetString() ?? errorMessage;
                                }
                            }
                            catch
                            {
                                // Si ce n'est pas du JSON, utiliser la chaîne directement
                                errorMessage = errorStr;
                            }
                        }
                    }
                    else if (errorJson.TryGetProperty("message", out var messageProp))
                    {
                        errorMessage = messageProp.GetString() ?? errorMessage;
                    }
                }
                catch
                {
                    // Si le parsing échoue, utiliser le contenu brut ou un message par défaut
                    if (!string.IsNullOrWhiteSpace(errorContent) && errorContent.Length < 200)
                    {
                        errorMessage = errorContent;
                    }
                }
                
                // Traduire certains messages d'erreur courants
                if (errorMessage.Contains("email") && errorMessage.Contains("invalid"))
                {
                    errorMessage = "L'adresse email n'est pas valide. Veuillez entrer une adresse email correcte (ex: nom@exemple.com)";
                }
                else if (errorMessage.Contains("password") && errorMessage.Contains("weak"))
                {
                    errorMessage = "Le mot de passe est trop faible. Veuillez choisir un mot de passe plus fort.";
                }
                else if (errorMessage.Contains("already exists") || errorMessage.Contains("already registered"))
                {
                    errorMessage = "Un compte avec cet email existe déjà. Veuillez vous connecter ou utiliser un autre email.";
                }
                
                throw new Exception(errorMessage);
            }
        }
        catch (HttpRequestException httpEx)
        {
            Debug.WriteLine($"AuthService: Erreur HTTP lors de l'inscription: {httpEx.Message}");
            Debug.WriteLine($"AuthService: Inner exception: {httpEx.InnerException?.Message}");
            if (httpEx.InnerException != null)
            {
                Debug.WriteLine($"AuthService: Inner exception type: {httpEx.InnerException.GetType().Name}");
            }
            throw new Exception($"Impossible de se connecter au serveur. Vérifiez que l'API est en cours d'exécution sur {_apiBaseUrl}. Erreur: {httpEx.Message}", httpEx);
        }
        catch (TaskCanceledException timeoutEx)
        {
            Debug.WriteLine($"AuthService: Timeout lors de l'inscription: {timeoutEx.Message}");
            throw new Exception($"La connexion au serveur a expiré. Vérifiez votre connexion réseau et que l'API est accessible sur {_apiBaseUrl}.", timeoutEx);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AuthService: Erreur lors de l'inscription: {ex.GetType().Name} - {ex.Message}");
            Debug.WriteLine($"AuthService: Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public void Logout()
    {
        SecureStorage.Default.Remove("auth_token");
        SecureStorage.Default.Remove("user_email");
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await SecureStorage.Default.GetAsync("auth_token");
        return !string.IsNullOrEmpty(token);
    }
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public object? User { get; set; }
    public int ExpiresIn { get; set; }
}