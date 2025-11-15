using System.Diagnostics;

namespace PlatformGame
{
    public partial class App : Application
    {
        public App()
        {
            try
            {
                Debug.WriteLine("App: Début de l'initialisation");
                InitializeComponent();
                Debug.WriteLine("App: Fin de l'initialisation");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"App: Erreur lors de l'initialisation: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            try
            {
                Debug.WriteLine("App: Création de la fenêtre");
                return new Window(new AppShell());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"App: Erreur lors de la création de la fenêtre: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}