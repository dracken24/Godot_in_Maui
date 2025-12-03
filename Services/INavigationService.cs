namespace PlatformGame.Services;

public interface INavigationService
{
    Task NavigateToAsync(string route);
    Task NavigateBackAsync();
    Task NavigateToMainPageAsync();
    Task NavigateToLoginPageAsync();
    Task NavigateToSignUpPageAsync();
}


