using System.Text.RegularExpressions;
using System.Windows.Input;
using PlatformGame.Services;

namespace PlatformGame.ViewModels;

public class SignUpPageViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private string _username = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private bool _isLoading;

    public SignUpPageViewModel(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;

        Title = "Inscription";
        SignUpCommand = new Command(async () => await SignUpAsync(), () => !IsBusy && IsFormValid());
        BackToLoginCommand = new Command(async () => await NavigateBackAsync(), () => !IsBusy);
    }

    public string Username
    {
        get => _username;
        set
        {
            SetProperty(ref _username, value);
            ((Command)SignUpCommand).ChangeCanExecute();
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            SetProperty(ref _email, value);
            ((Command)SignUpCommand).ChangeCanExecute();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            SetProperty(ref _password, value);
            ((Command)SignUpCommand).ChangeCanExecute();
        }
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set
        {
            SetProperty(ref _confirmPassword, value);
            ((Command)SignUpCommand).ChangeCanExecute();
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            SetProperty(ref _isLoading, value);
            IsBusy = value;
            ((Command)SignUpCommand).ChangeCanExecute();
            ((Command)BackToLoginCommand).ChangeCanExecute();
        }
    }

    public ICommand SignUpCommand { get; }
    public ICommand BackToLoginCommand { get; }

    private bool IsFormValid()
    {
        return !string.IsNullOrWhiteSpace(Username) &&
               !string.IsNullOrWhiteSpace(Email) &&
               !string.IsNullOrWhiteSpace(Password) &&
               !string.IsNullOrWhiteSpace(ConfirmPassword) &&
               IsValidEmail(Email) &&
               Password.Length >= 6 &&
               Password == ConfirmPassword;
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
        return EmailRegex.IsMatch(email);
    }

    private async Task SignUpAsync()
    {
        if (!IsFormValid())
        {
            if (!IsValidEmail(Email))
            {
                await Application.Current!.MainPage!.DisplayAlert("Erreur", "Veuillez entrer une adresse email valide (ex: nom@exemple.com)", "OK");
                return;
            }

            if (Password.Length < 6)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erreur", "Le mot de passe doit contenir au moins 6 caractères", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erreur", "Les mots de passe ne correspondent pas", "OK");
                return;
            }

            await Application.Current!.MainPage!.DisplayAlert("Erreur", "Veuillez remplir tous les champs", "OK");
            return;
        }

        try
        {
            IsLoading = true;

            var success = await _authService.RegisterAsync(Email, Password, Username);

            if (success)
            {
                // Connecter automatiquement l'utilisateur après l'inscription
                var loginResult = await _authService.LoginAsync(Email, Password);

                if (loginResult != null)
                {
                    await Application.Current!.MainPage!.DisplayAlert("Succès", "Compte créé avec succès !", "OK");
                    await _navigationService.NavigateToMainPageAsync();
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert("Succès", "Compte créé avec succès ! Veuillez vous connecter.", "OK");
                    await NavigateBackAsync();
                }
            }
            else
            {
                await Application.Current!.MainPage!.DisplayAlert("Erreur", "L'inscription a échoué. Veuillez réessayer.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Erreur", $"Une erreur est survenue: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task NavigateBackAsync()
    {
        await _navigationService.NavigateBackAsync();
    }
}


