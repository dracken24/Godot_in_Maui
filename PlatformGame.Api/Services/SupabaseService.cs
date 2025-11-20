using Supabase;

namespace PlatformGame.Api.Services;

public class SupabaseService
{
    private readonly Client _client;
    private readonly ILogger<SupabaseService> _logger;

    public SupabaseService(IConfiguration configuration, ILogger<SupabaseService> logger)
    {
        _logger = logger;
        
        // Lire depuis les variables d'environnement (chargées depuis .env) ou depuis appsettings.json
        var url = Environment.GetEnvironmentVariable("SUPABASE_URL") ?? configuration["Supabase:Url"];
        var anonKey = Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY") ?? configuration["Supabase:AnonKey"];

        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(anonKey))
        {
            _logger.LogWarning("Configuration Supabase manquante. Utilisez .env ou appsettings.json pour configurer l'URL et la clé.");
            throw new InvalidOperationException("Configuration Supabase manquante. Veuillez configurer SUPABASE_URL et SUPABASE_ANON_KEY dans .env ou Supabase:Url et Supabase:AnonKey dans appsettings.json");
        }

        var options = new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = false
        };

        _client = new Client(url, anonKey, options);
    }

    public Client GetClient() => _client;
}

