using System.Windows.Input;
using PlatformGame.Services;

namespace PlatformGame.ViewModels;

public class LoginPageViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    private string _email = string.Empty;
    private string _password = string.Empty;
    private bool _isLoading;

    public LoginPageViewModel(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;

        Title = "Connexion";
        LoginCommand = new Command(async () => await LoginAsync(), () => !IsBusy && !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password));
        SignUpCommand = new Command(async () => await NavigateToSignUpAsync(), () => !IsBusy);
    }

    public string Email
    {
        get => _email;
        set
        {
            SetProperty(ref _email, value);
            ((Command)LoginCommand).ChangeCanExecute();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            SetProperty(ref _password, value);
            ((Command)LoginCommand).ChangeCanExecute();
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            SetProperty(ref _isLoading, value);
            IsBusy = value;
            ((Command)LoginCommand).ChangeCanExecute();
            ((Command)SignUpCommand).ChangeCanExecute();
        }
    }

    public ICommand LoginCommand { get; }
    public ICommand SignUpCommand { get; }

    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current!.MainPage!.DisplayAlert("Erreur", "Veuillez remplir tous les champs", "OK");
            return;
        }

        try
        {
            IsLoading = true;

            var result = await _authService.LoginAsync(Email, Password);

            if (result != null)
            {
                await _navigationService.NavigateToMainPageAsync();
            }
            else
            {
                await Application.Current!.MainPage!.DisplayAlert("Erreur", "Email ou mot de passe incorrect", "OK");
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

    private async Task NavigateToSignUpAsync()
    {
        await _navigationService.NavigateToSignUpPageAsync();
    }
}


