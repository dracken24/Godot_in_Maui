namespace PlatformGame.Api.Models;

public class CreateGameResultRequest
{
    public long CompletionTimeMs { get; set; }
    public string? PlayerName { get; set; }
    public string? Platform { get; set; }
}

