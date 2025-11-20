using PlatformGame.Services;
using System.Text.RegularExpressions;
using PlatformGame;

namespace PlatformGame.Views;

public partial class SignUpPage : ContentPage
{
    private readonly AuthService _authService;
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public SignUpPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    public SignUpPage() : this(new AuthService())
    {
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
        
        return EmailRegex.IsMatch(email);
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UsernameEntry.Text) || 
            string.IsNullOrWhiteSpace(EmailEntry.Text) || 
            string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez remplir tous les champs", "OK");
            return;
        }

        if (!IsValidEmail(EmailEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez entrer une adresse email valide (ex: nom@exemple.com)", "OK");
            return;
        }

        if (PasswordEntry.Text.Length < 6)
        {
            await DisplayAlert("Erreur", "Le mot de passe doit contenir au moins 6 caractères", "OK");
            return;
        }

        if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            await DisplayAlert("Erreur", "Les mots de passe ne correspondent pas", "OK");
            return;
        }

        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;
        UsernameEntry.IsEnabled = false;
        EmailEntry.IsEnabled = false;
        PasswordEntry.IsEnabled = false;
        ConfirmPasswordEntry.IsEnabled = false;

        try
        {
            var success = await _authService.RegisterAsync(EmailEntry.Text, PasswordEntry.Text, UsernameEntry.Text);
            
            if (success)
            {
                // Connecter automatiquement l'utilisateur après l'inscription
                var loginResult = await _authService.LoginAsync(EmailEntry.Text, PasswordEntry.Text);
                
                if (loginResult != null)
                {
                    await DisplayAlert("Succès", "Compte créé avec succès !", "OK");
                    
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
                    await DisplayAlert("Succès", "Compte créé avec succès ! Veuillez vous connecter.", "OK");
                    await Navigation.PopAsync();
                }
            }
            else
            {
                await DisplayAlert("Erreur", "L'inscription a échoué. Veuillez réessayer.", "OK");
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
            UsernameEntry.IsEnabled = true;
            EmailEntry.IsEnabled = true;
            PasswordEntry.IsEnabled = true;
            ConfirmPasswordEntry.IsEnabled = true;
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
