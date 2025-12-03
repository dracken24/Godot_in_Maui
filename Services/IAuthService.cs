namespace PlatformGame.Services;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(string email, string password, string username);
    void Logout();
    Task<bool> IsAuthenticatedAsync();
    Task<string?> GetUserEmailAsync();
}


