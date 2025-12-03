using PlatformGame.Services;
using PlatformGame.Views;

namespace PlatformGame;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Enregistrement des routes
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
    }
    
    protected override async void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        
        // Vérifier l'authentification une fois que le Handler est initialisé
        if (Handler != null)
        {
            await CheckAuthenticationAndRedirect();
        }
    }
    
    private async Task CheckAuthenticationAndRedirect()
    {
        try
        {
            // Attendre un peu pour que le Handler soit complètement initialisé
            await Task.Delay(200);
            
            var authService = Handler?.MauiContext?.Services.GetService<IAuthService>();
            if (authService != null)
            {
                var isAuthenticated = await authService.IsAuthenticatedAsync();
                if (isAuthenticated)
                {
                    // Rediriger vers la page principale si authentifié
                    var currentLocation = CurrentState?.Location.OriginalString ?? "";
                    if (currentLocation.Contains("LoginPage") || currentLocation.Contains("SignUpPage") || string.IsNullOrEmpty(currentLocation))
                    {
                        await GoToAsync($"//{nameof(MainPage)}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AppShell: Erreur lors de la vérification de l'authentification: {ex.Message}");
        }
    }

    protected override async void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);

        // Protection de la route MainPage
        if (args.Target.Location.OriginalString.Contains(nameof(MainPage)))
        {
            var authService = Handler?.MauiContext?.Services.GetService<IAuthService>();
            if (authService != null)
            {
                var isAuthenticated = await authService.IsAuthenticatedAsync();
                if (!isAuthenticated)
                {
                    args.Cancel();
                    await DisplayAlert("Accès refusé", "Veuillez vous connecter pour accéder au jeu.", "OK");
                    await GoToAsync($"//{nameof(LoginPage)}");
                }
            }
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        var authService = Handler?.MauiContext?.Services.GetService<IAuthService>();
        authService?.Logout();
        
        await DisplayAlert("Déconnexion", "Vous avez été déconnecté.", "OK");
        await GoToAsync($"//{nameof(LoginPage)}");
    }
}