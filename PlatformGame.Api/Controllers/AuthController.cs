using Microsoft.AspNetCore.Mvc;
using PlatformGame.Api.Services;
using Supabase.Gotrue;

namespace PlatformGame.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SupabaseService _supabaseService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(SupabaseService supabaseService, ILogger<AuthController> logger)
    {
        _supabaseService = supabaseService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var client = _supabaseService.GetClient();
            
            var session = await client.Auth.SignUp(request.Email, request.Password, new SignUpOptions
            {
                Data = new Dictionary<string, object>
                {
                    { "username", request.Username }
                }
            });

            if (session?.User == null)
            {
                return BadRequest("Inscription échouée. Vérifiez les données.");
            }

            return Ok(new { message = "Inscription réussie", user = session.User });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'inscription");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var client = _supabaseService.GetClient();
            
            var session = await client.Auth.SignIn(request.Email, request.Password);

            if (session?.User == null || string.IsNullOrEmpty(session.AccessToken))
            {
                return Unauthorized(new { error = "Échec de la connexion. Vérifiez vos identifiants." });
            }

            return Ok(new 
            { 
                token = session.AccessToken,
                refreshToken = session.RefreshToken,
                user = session.User,
                expiresIn = session.ExpiresIn
            });
        }
        catch (Supabase.Gotrue.Exceptions.GotrueException gotrueEx)
        {
            _logger.LogError(gotrueEx, "Erreur Gotrue lors de la connexion");
            
            // Gérer les erreurs spécifiques de Supabase
            string errorMessage = "Email ou mot de passe incorrect";
            
            if (gotrueEx.Message.Contains("email_not_confirmed") || gotrueEx.Message.Contains("Email not confirmed"))
            {
                errorMessage = "Votre adresse email n'a pas été confirmée. Veuillez vérifier votre boîte mail et cliquer sur le lien de confirmation.";
            }
            else if (gotrueEx.Message.Contains("Invalid login credentials") || gotrueEx.Message.Contains("invalid"))
            {
                errorMessage = "Email ou mot de passe incorrect";
            }
            
            return Unauthorized(new { error = errorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la connexion");
            return Unauthorized(new { error = "Une erreur est survenue lors de la connexion. Veuillez réessayer." });
        }
    }
}

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
