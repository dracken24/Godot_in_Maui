namespace PlatformGame.Api.Models;

public class CreateGameResultRequest
{
    public long CompletionTimeMs { get; set; }
    public string? Username { get; set; }
    public string? Platform { get; set; }
}

