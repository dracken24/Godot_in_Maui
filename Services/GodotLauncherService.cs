using System.Diagnostics;

namespace PlatformGame.Services;

public class GodotLauncherService : IGodotLauncherService
{
    public async Task LaunchGodotAsync()
    {
        try
        {
#if WINDOWS
            LaunchGodotWindows();
#elif MACCATALYST
            LaunchGodotMac();
#elif LINUX
            LaunchGodotLinux();
#elif ANDROID
            LaunchGodotAndroid();
#endif
        }
        catch (Exception)
        {
            throw;
        }
    }

    private void LaunchGodotWindows()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Godot", "MyGodotGame.exe");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Impossible de trouver le jeu Godot.", path);
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true
        });
    }

    private void LaunchGodotMac()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Godot", "MyGodotGame.app");
        Process.Start("open", $"\"{path}\"");
    }

    private void LaunchGodotLinux()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Godot", "MyGodotGame.x86_64");

        Process.Start(new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true
        });
    }

#if ANDROID
    private void LaunchGodotAndroid()
    {
        var packageName = "com.company.mygodotgame";
        var activityName = "com.godot.game.GodotApp";
        
        try
        {
            var packageManager = Android.App.Application.Context.PackageManager;
            if (packageManager == null)
            {
                throw new InvalidOperationException("Impossible d'accéder au gestionnaire de packages.");
            }

            var intent = packageManager.GetLaunchIntentForPackage(packageName);

            if (intent == null)
            {
                try
                {
                    intent = new Android.Content.Intent(Android.Content.Intent.ActionMain);
                    intent.SetPackage(packageName);
                    intent.AddCategory(Android.Content.Intent.CategoryLauncher);
                    intent.SetComponent(new Android.Content.ComponentName(packageName, activityName));
                    intent.SetFlags(Android.Content.ActivityFlags.NewTask | Android.Content.ActivityFlags.ResetTaskIfNeeded);
                }
                catch (Exception ex2)
                {
                    intent = new Android.Content.Intent(Android.Content.Intent.ActionMain);
                    intent.SetPackage(packageName);
                    intent.AddCategory(Android.Content.Intent.CategoryLauncher);
                    intent.SetFlags(Android.Content.ActivityFlags.NewTask | Android.Content.ActivityFlags.ResetTaskIfNeeded);
                }
            }

            if (intent == null)
            {
                throw new InvalidOperationException($"Application Godot ({packageName}) non trouvée ou non installée.");
            }

            intent.AddFlags(Android.Content.ActivityFlags.NewTask | Android.Content.ActivityFlags.ClearTop);
            Android.App.Application.Context.StartActivity(intent);
            
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current!.MainPage!.DisplayAlert("Jeu lancé", 
                    "Le jeu Godot a été lancé.\n\n" +
                    "L'app MAUI reste active en arrière-plan.\n" +
                    "Vous pouvez y revenir via le gestionnaire de tâches Android.", 
                    "OK");
            });
        }
        catch (Android.Content.ActivityNotFoundException)
        {
            throw new InvalidOperationException(
                $"L'application Godot ({packageName}) est installée mais ne peut pas être lancée.\n\n" +
                "Causes possibles:\n" +
                "• L'APK Godot est mal exporté\n" +
                "• L'activité principale n'est pas déclarée dans AndroidManifest.xml\n" +
                "• L'APK est corrompu\n\n" +
                "Vérifiez l'export depuis Godot et réinstallez l'APK.");
        }
    }
#endif
}
