using System.Diagnostics;
using PlatformGame.Services;
using System.Text.Json;

namespace PlatformGame;

public partial class MainPage : ContentPage
{
    private readonly GameApiService _gameApiService;

    public MainPage(GameApiService gameApiService)
    {
        try
        {
            Debug.WriteLine("MainPage: Début de l'initialisation");
            _gameApiService = gameApiService;
            InitializeComponent();
            Debug.WriteLine("MainPage: Fin de l'initialisation");
            
            // Charger l'image de manière asynchrone pour améliorer les performances
            LoadImageAsync();
            
            // Charger les résultats au démarrage (fire and forget)
            _ = LoadResultsAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"MainPage: Erreur lors de l'initialisation: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    private async void LoadImageAsync()
    {
        try
        {
            Debug.WriteLine("LoadImageAsync: Début du chargement de l'image");
            // L'image est déjà définie dans le XAML, cette méthode peut être utilisée
            // pour précharger ou vérifier la disponibilité de l'image
            await Task.Delay(100); // Petit délai pour permettre au reste de l'UI de se charger
            Debug.WriteLine("LoadImageAsync: Image chargée");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"LoadImageAsync: Erreur: {ex.Message}");
        }
    }

    private async Task LoadResultsAsync()
    {
        try
        {
            LoadingIndicator.IsRunning = true;
            LoadingIndicator.IsVisible = true;

            Debug.WriteLine("LoadResultsAsync: Début du chargement des résultats");

            // Charger les statistiques
            try
            {
                var stats = await _gameApiService.GetStatsAsync();
                if (stats != null && stats.TotalAttempts > 0)
                {
                    StatsLabel.Text = $"Total de tentatives: {stats.TotalAttempts}\n" +
                                     $"Temps moyen: {stats.AverageTimeFormatted}\n" +
                                     $"Meilleur temps: {stats.BestTimeFormatted}";
                }
                else
                {
                    StatsLabel.Text = "Aucune statistique disponible\n(Aucun résultat enregistré)";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadResultsAsync: Erreur lors du chargement des statistiques: {ex.Message}");
                StatsLabel.Text = "Aucune statistique disponible";
            }

            // Charger le meilleur temps
            try
            {
                var bestResult = await _gameApiService.GetBestResultAsync();
                if (bestResult != null)
                {
                    var email = string.IsNullOrEmpty(bestResult.Email) ? "Anonyme" : bestResult.Email;
                    BestTimeLabel.Text = $"{bestResult.CompletionTimeFormatted}\n" +
                                        $"Par: {email}\n" +
                                        $"Plateforme: {bestResult.Platform ?? "N/A"}\n" +
                                        $"Date: {bestResult.CreatedAt:dd/MM/yyyy HH:mm}";
                }
                else
                {
                    BestTimeLabel.Text = "Aucun résultat enregistré";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadResultsAsync: Erreur lors du chargement du meilleur résultat: {ex.Message}");
                BestTimeLabel.Text = "Aucun résultat enregistré";
            }

            // Charger le top 10
            try
            {
                var topResults = await _gameApiService.GetTopResultsAsync(10);
                if (topResults != null && topResults.Any())
                {
                    var topResultsText = string.Join("\n", topResults.Select((r, index) =>
                    {
                        var email = string.IsNullOrEmpty(r.Email) ? "Anonyme" : r.Email;
                        return $"{index + 1}. {r.CompletionTimeFormatted} - {email} ({r.Platform ?? "N/A"})";
                    }));
                    TopResultsLabel.Text = topResultsText;
                }
                else
                {
                    TopResultsLabel.Text = "Aucun résultat disponible";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadResultsAsync: Erreur lors du chargement du top 10: {ex.Message}");
                TopResultsLabel.Text = "Aucun résultat disponible";
            }

            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;

            Debug.WriteLine("LoadResultsAsync: Fin du chargement des résultats");
        }
        catch (Exception ex)
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
            Debug.WriteLine($"LoadResultsAsync: Erreur: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await DisplayAlert("Erreur", 
                    $"Impossible de charger les résultats: {ex.Message}\n\n" +
                    "Vérifiez que l'API est démarrée et accessible.", 
                    "OK");
            });
        }
    }

    private async void OnRefreshResultsClicked(object sender, EventArgs e)
    {
        await LoadResultsAsync();
    }

    private async void OnLaunchGodotClicked(object sender, EventArgs e)
    {
        try
        {
            Debug.WriteLine("OnLaunchGodotClicked: Début");
            
            // Récupérer l'email de l'utilisateur connecté
            var userEmail = await SecureStorage.Default.GetAsync("user_email");
            
            if (string.IsNullOrEmpty(userEmail))
            {
                await DisplayAlert("Erreur", "Vous devez être connecté pour lancer le jeu.", "OK");
                return;
            }
            
            Debug.WriteLine($"OnLaunchGodotClicked: Email de l'utilisateur: {userEmail}");
            
            // Créer un fichier de configuration partagé avec l'email
            // IMPORTANT: Ce fichier est recréé à CHAQUE lancement pour garantir
            // que le bon email (celui de l'utilisateur actuellement connecté) est utilisé
            // ATTENDRE que le fichier soit créé et copié avant de lancer le jeu
            await CreateSharedConfigFile(userEmail);
            
            // Attendre un peu plus pour s'assurer que le fichier est bien copié et vérifié
            // Le délai est important car la copie via ADB peut prendre du temps
            await Task.Delay(1500);
            
#if WINDOWS
            LaunchGodotWindows(userEmail);
#elif MACCATALYST
            LaunchGodotMac(userEmail);
#elif LINUX
            LaunchGodotLinux(userEmail);
#elif ANDROID
            LaunchGodotAndroid(userEmail);
#endif
            Debug.WriteLine("OnLaunchGodotClicked: Fin");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"OnLaunchGodotClicked: Erreur: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            _ = DisplayAlert("Erreur", $"Impossible de lancer le jeu: {ex.Message}", "OK");
        }
    }
    
    /// <summary>
    /// Crée un fichier de configuration partagé que Godot peut lire
    /// </summary>
    private async Task CreateSharedConfigFile(string email)
    {
        try
        {
            var config = new
            {
                email = email,
                apiBaseUrl = Environment.GetEnvironmentVariable("API_BASE_URL") ?? "http://10.0.0.49:5000",
                timestamp = DateTime.UtcNow
            };
            
            var json = JsonSerializer.Serialize(config);
            
#if ANDROID
            // Sur Android, créer le fichier dans plusieurs emplacements pour maximiser les chances
            // NOTE: Le plugin Android Intent Extras est maintenant la méthode principale,
            // mais on garde le fichier comme solution de secours
            var paths = new List<string>();
            
            // NOTE: Le plugin Android Intent Extras est maintenant la méthode principale.
            // Le fichier est créé uniquement comme solution de secours.
            
            // 1. Dossier Download (accessible sans permissions spéciales sur certaines versions)
            var downloadPath = Path.Combine(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryDownloads)?.AbsolutePath ?? 
                "/sdcard/Download",
                "platformgame_config.json");
            paths.Add(downloadPath);
            
            // 4. Dossier Documents (emplacement original)
            var documentsPath = Path.Combine(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryDocuments)?.AbsolutePath ?? 
                "/storage/emulated/0/Documents",
                "platformgame_config.json");
            paths.Add(documentsPath);
            
            // Créer le fichier dans tous les emplacements possibles
            foreach (var filePath in paths)
            {
                try
                {
                    // Créer le répertoire parent s'il n'existe pas
                    var dir = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    
                    await File.WriteAllTextAsync(filePath, json);
                    Debug.WriteLine($"CreateSharedConfigFile: Fichier créé à {filePath}");
                }
                catch (Exception pathEx)
                {
                    Debug.WriteLine($"CreateSharedConfigFile: Erreur pour {filePath}: {pathEx.Message}");
                }
            }
            
            // IMPORTANT: L'email est maintenant transmis via Intent extras (plugin Android)
            // Le fichier est créé comme solution de secours uniquement
            Debug.WriteLine($"CreateSharedConfigFile: Fichier créé pour l'email: {email}");
            Debug.WriteLine($"CreateSharedConfigFile: L'email est transmis via Intent extras (plugin Android)");
            Debug.WriteLine($"CreateSharedConfigFile: Le fichier est créé comme solution de secours");
            Debug.WriteLine($"CreateSharedConfigFile: Contenu: {json}");
#else
            // Pour Windows/Mac/Linux, utiliser le dossier temporaire
            var filePath = Path.Combine(Path.GetTempPath(), "platformgame_config.json");
            await File.WriteAllTextAsync(filePath, json);
            Debug.WriteLine($"CreateSharedConfigFile: Fichier créé à {filePath}");
            Debug.WriteLine($"CreateSharedConfigFile: Contenu: {json}");
#endif
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"CreateSharedConfigFile: Erreur lors de la création du fichier: {ex.Message}");
            // Ne pas bloquer le lancement si le fichier ne peut pas être créé
        }
    }

    void LaunchGodotWindows(string email)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Godot", "MyGodotGame.exe");

        if (!File.Exists(path))
        {
            DisplayAlert("Erreur", "Impossible de trouver le jeu Godot.", "OK");
            return;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = path,
            Arguments = $"--email \"{email}\"",
            UseShellExecute = true
        });
    }

    void LaunchGodotMac(string email)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Godot", "MyGodotGame.app");

        // Sur Mac, on peut passer l'email via des arguments ou un fichier de configuration
        // Le fichier de configuration est déjà créé par CreateSharedConfigFile
        Process.Start("open", $"\"{path}\"");
    }

    void LaunchGodotLinux(string email)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Godot", "MyGodotGame.x86_64");

        Process.Start(new ProcessStartInfo
        {
            FileName = path,
            Arguments = $"--email \"{email}\"",
            UseShellExecute = true
        });
    }

#if ANDROID

    void LaunchGodotAndroid(string email)
    {
        try
        {
            Debug.WriteLine("LaunchGodotAndroid: Début");
            var packageName = "com.company.mygodotgame";
            var activityName = "com.godot.game.GodotApp"; // Activité principale de Godot
            
            Debug.WriteLine($"LaunchGodotAndroid: Recherche du package {packageName}");
            Debug.WriteLine($"LaunchGodotAndroid: Email à transmettre: {email}");

            var packageManager = Android.App.Application.Context.PackageManager;
            if (packageManager == null)
            {
                Debug.WriteLine("LaunchGodotAndroid: PackageManager est null");
                DisplayAlert("Erreur", "Impossible d'accéder au gestionnaire de packages.", "OK");
                return;
            }

            // Méthode 1: Essayer GetLaunchIntentForPackage
            var intent = packageManager.GetLaunchIntentForPackage(packageName);

            if (intent == null)
            {
                Debug.WriteLine($"LaunchGodotAndroid: GetLaunchIntentForPackage retourne null, tentative avec l'activité explicite");
                
                // Méthode 2: Créer un Intent explicite avec l'activité principale
                try
                {
                    intent = new Android.Content.Intent(Android.Content.Intent.ActionMain);
                    intent.SetPackage(packageName);
                    intent.AddCategory(Android.Content.Intent.CategoryLauncher);
                    intent.SetComponent(new Android.Content.ComponentName(packageName, activityName));
                    intent.SetFlags(Android.Content.ActivityFlags.NewTask | Android.Content.ActivityFlags.ResetTaskIfNeeded);
                    
                    Debug.WriteLine($"LaunchGodotAndroid: Intent créé avec ComponentName: {packageName}/{activityName}");
                }
                catch (Exception ex2)
                {
                    Debug.WriteLine($"LaunchGodotAndroid: Erreur lors de la création de l'Intent explicite: {ex2.Message}");
                    
                    // Méthode 3: Intent générique avec ACTION_MAIN (sans ComponentName)
                    intent = new Android.Content.Intent(Android.Content.Intent.ActionMain);
                    intent.SetPackage(packageName);
                    intent.AddCategory(Android.Content.Intent.CategoryLauncher);
                    intent.SetFlags(Android.Content.ActivityFlags.NewTask | Android.Content.ActivityFlags.ResetTaskIfNeeded);
                    
                    Debug.WriteLine($"LaunchGodotAndroid: Intent générique créé pour le package {packageName}");
                }
            }

            if (intent == null)
            {
                Debug.WriteLine($"LaunchGodotAndroid: Impossible de créer un Intent pour {packageName}");
                DisplayAlert("Erreur", $"Application Godot ({packageName}) non trouvée ou non installée.", "OK");
                return;
            }

            // Ajouter l'email comme extra dans l'Intent
            intent.PutExtra("user_email", email);
            intent.PutExtra("api_base_url", Environment.GetEnvironmentVariable("API_BASE_URL") ?? "http://10.0.0.49:5000");
            Debug.WriteLine($"LaunchGodotAndroid: Email ajouté comme extra dans l'Intent: {email}");

            // Vérifier si l'activité peut être résolue (mais ne pas bloquer si ça échoue)
            var resolveInfo = packageManager.ResolveActivity(intent, Android.Content.PM.PackageInfoFlags.MatchDefaultOnly);
            if (resolveInfo == null)
            {
                Debug.WriteLine($"LaunchGodotAndroid: ATTENTION - L'activité ne peut pas être résolue pour {packageName}");
                Debug.WriteLine($"LaunchGodotAndroid: Cela peut indiquer que l'APK Godot est mal exporté ou corrompu");
                Debug.WriteLine($"LaunchGodotAndroid: Tentative de lancement quand même...");
                
                // Essayer quand même avec un Intent générique sans ComponentName
                intent = new Android.Content.Intent(Android.Content.Intent.ActionMain);
                intent.SetPackage(packageName);
                intent.AddCategory(Android.Content.Intent.CategoryLauncher);
                intent.SetFlags(Android.Content.ActivityFlags.NewTask | Android.Content.ActivityFlags.ResetTaskIfNeeded);
                // Réajouter les extras
                intent.PutExtra("user_email", email);
                intent.PutExtra("api_base_url", Environment.GetEnvironmentVariable("API_BASE_URL") ?? "http://10.0.0.49:5000");
            }

            Debug.WriteLine($"LaunchGodotAndroid: Lancement de l'application {packageName}");
            try
            {
                // Utiliser NewTask pour lancer dans une nouvelle tâche sans fermer l'app MAUI
                intent.AddFlags(Android.Content.ActivityFlags.NewTask | Android.Content.ActivityFlags.ClearTop);
                
                // Lancer l'app Godot
                Android.App.Application.Context.StartActivity(intent);
                
                Debug.WriteLine("LaunchGodotAndroid: Application lancée avec succès");
                Debug.WriteLine("LaunchGodotAndroid: L'app MAUI reste en arrière-plan et peut être relancée via le gestionnaire de tâches");
                
                // Afficher un message informatif
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Jeu lancé", 
                        "Le jeu Godot a été lancé.\n\n" +
                        "L'app MAUI reste active en arrière-plan.\n" +
                        "Vous pouvez y revenir via le gestionnaire de tâches Android.", 
                        "OK");
                });
            }
            catch (Android.Content.ActivityNotFoundException)
            {
                Debug.WriteLine("LaunchGodotAndroid: ActivityNotFoundException - L'app Godot n'a pas d'activité principale valide");
                DisplayAlert("Erreur", 
                    $"L'application Godot ({packageName}) est installée mais ne peut pas être lancée.\n\n" +
                    "Causes possibles:\n" +
                    "• L'APK Godot est mal exporté\n" +
                    "• L'activité principale n'est pas déclarée dans AndroidManifest.xml\n" +
                    "• L'APK est corrompu\n\n" +
                    "Vérifiez l'export depuis Godot et réinstallez l'APK.", 
                    "OK");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"LaunchGodotAndroid: Erreur: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            DisplayAlert("Erreur", $"Impossible de lancer le jeu: {ex.Message}", "OK");
        }
    }
#endif
}
