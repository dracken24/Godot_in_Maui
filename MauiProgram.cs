using Microsoft.Extensions.Logging;
using System.Diagnostics;
using PlatformGame.Services;
using DotNetEnv;

namespace PlatformGame
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            try
            {
                Debug.WriteLine("MauiProgram: Début de la création de l'application");
                
                // Charger le fichier .env s'il existe (cherche à la racine du projet)
                var envPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".env");
                if (File.Exists(envPath))
                {
                    Env.Load(envPath);
                    Debug.WriteLine($"MauiProgram: Fichier .env chargé depuis {envPath}");
                }
                else
                {
                    // Essayer aussi à la racine du projet (pour le développement)
                    var rootEnvPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
                    if (File.Exists(rootEnvPath))
                    {
                        Env.Load(rootEnvPath);
                        Debug.WriteLine($"MauiProgram: Fichier .env chargé depuis {rootEnvPath}");
                    }
                    else
                    {
                        // Essayer aussi depuis le dossier parent (si on est dans bin/Debug)
                        var parentEnvPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".env");
                        if (File.Exists(parentEnvPath))
                        {
                            Env.Load(parentEnvPath);
                            Debug.WriteLine($"MauiProgram: Fichier .env chargé depuis {parentEnvPath}");
                        }
                        else
                        {
                            Debug.WriteLine("MauiProgram: Aucun fichier .env trouvé, utilisation des valeurs par défaut");
                        }
                    }
                }
                
                // Afficher l'URL de l'API configurée
                var apiUrl = Environment.GetEnvironmentVariable("API_BASE_URL");
                if (!string.IsNullOrEmpty(apiUrl))
                {
                    Debug.WriteLine($"MauiProgram: API_BASE_URL depuis .env: {apiUrl}");
                }
                else
                {
                    Debug.WriteLine("MauiProgram: API_BASE_URL non défini dans .env, utilisation de la valeur par défaut");
                }
                
                // Gestion globale des exceptions non gérées au niveau de l'AppDomain
                AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    Debug.WriteLine($"Exception non gérée dans AppDomain: {e.ExceptionObject}");
                    if (e.ExceptionObject is Exception ex)
                    {
                        Debug.WriteLine($"Message: {ex.Message}");
                        Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    }
                };

                var builder = MauiApp.CreateBuilder();
                builder
                    .UseMauiApp<App>()
                    .ConfigureFonts(fonts =>
                    {
                        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    });

#if DEBUG
                builder.Logging.AddDebug();
#endif

                // Enregistrer les services (interfaces et implémentations)
                builder.Services.AddSingleton<IAuthService, AuthService>();
                builder.Services.AddSingleton<IGameApiService, GameApiService>();
                builder.Services.AddSingleton<INavigationService, NavigationService>();
                builder.Services.AddSingleton<IGodotLauncherService, GodotLauncherService>();

                // Enregistrer les ViewModels
                builder.Services.AddTransient<ViewModels.MainPageViewModel>();
                builder.Services.AddTransient<ViewModels.LoginPageViewModel>();
                builder.Services.AddTransient<ViewModels.SignUpPageViewModel>();

                // Enregistrer les pages avec leurs ViewModels
                builder.Services.AddTransient<MainPage>(serviceProvider =>
                {
                    var viewModel = serviceProvider.GetRequiredService<ViewModels.MainPageViewModel>();
                    return new MainPage(viewModel);
                });
                builder.Services.AddTransient<Views.LoginPage>(serviceProvider =>
                {
                    var viewModel = serviceProvider.GetRequiredService<ViewModels.LoginPageViewModel>();
                    return new Views.LoginPage(viewModel);
                });
                builder.Services.AddTransient<Views.SignUpPage>(serviceProvider =>
                {
                    var viewModel = serviceProvider.GetRequiredService<ViewModels.SignUpPageViewModel>();
                    return new Views.SignUpPage(viewModel);
                });

                var app = builder.Build();
                Debug.WriteLine("MauiProgram: Application créée avec succès");
                return app;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MauiProgram: Erreur lors de la création de l'application: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
