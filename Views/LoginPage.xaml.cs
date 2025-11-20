using PlatformGame.Services;
using PlatformGame;

namespace PlatformGame.Views;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;

    public LoginPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    public LoginPage() : this(new AuthService())
    {
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez remplir tous les champs", "OK");
            return;
        }

        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;
        EmailEntry.IsEnabled = false;
        PasswordEntry.IsEnabled = false;

        try
        {
            var result = await _authService.LoginAsync(EmailEntry.Text, PasswordEntry.Text);
            
            if (result != null)
            {
                // Naviguer vers la page principale (jeu)
                if (Application.Current?.MainPage is Shell shell)
                {
                    await shell.GoToAsync($"//{nameof(MainPage)}");
                }
                else
                {
                    // Si on n'est pas dans un Shell, créer un nouveau AppShell et naviguer
                    Application.Current.MainPage = new AppShell();
                    await ((AppShell)Application.Current.MainPage).GoToAsync($"//{nameof(MainPage)}");
                }
            }
            else
            {
                await DisplayAlert("Erreur", "Email ou mot de passe incorrect", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Une erreur est survenue: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
            EmailEntry.IsEnabled = true;
            PasswordEntry.IsEnabled = true;
        }
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SignUpPage(new AuthService()));
    }
}
