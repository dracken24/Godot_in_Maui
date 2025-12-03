namespace PlatformGame.Services;

public class NavigationService : INavigationService
{
    public async Task NavigateToAsync(string route)
    {
        if (Application.Current?.MainPage is Shell shell)
        {
            await shell.GoToAsync(route);
        }
    }

    public async Task NavigateBackAsync()
    {
        if (Application.Current?.MainPage is Page page)
        {
            await page.Navigation.PopAsync();
        }
    }

    public async Task NavigateToMainPageAsync()
    {
        await NavigateToAsync($"//{nameof(MainPage)}");
    }

    public async Task NavigateToLoginPageAsync()
    {
        await NavigateToAsync($"//{nameof(Views.LoginPage)}");
    }

    public async Task NavigateToSignUpPageAsync()
    {
        await NavigateToAsync(nameof(Views.SignUpPage));
    }
}


