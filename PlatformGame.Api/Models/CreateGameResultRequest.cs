namespace PlatformGame.Api.Models;

public class CreateGameResultRequest
{
    public long CompletionTimeMs { get; set; }
    public string? Email { get; set; }
    public string? Platform { get; set; }
}

