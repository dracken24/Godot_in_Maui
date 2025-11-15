using Android.App;
using Android.Content.PM;
using Android.OS;
using System.Diagnostics;

namespace PlatformGame
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnPause()
        {
            base.OnPause();
            // Empêcher l'app de se fermer complètement quand une autre activité démarre
            System.Diagnostics.Debug.WriteLine("MainActivity: OnPause - L'app passe en arrière-plan");
        }

        protected override void OnResume()
        {
            base.OnResume();
            System.Diagnostics.Debug.WriteLine("MainActivity: OnResume - L'app revient au premier plan");
        }

        protected override void OnStop()
        {
            base.OnStop();
            System.Diagnostics.Debug.WriteLine("MainActivity: OnStop - L'app s'arrête mais reste en mémoire");
        }
    }
}
