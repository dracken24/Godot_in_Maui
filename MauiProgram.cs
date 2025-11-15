using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace PlatformGame
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            try
            {
                Debug.WriteLine("MauiProgram: Début de la création de l'application");
                
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
