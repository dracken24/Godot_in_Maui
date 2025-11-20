namespace PlatformGame.Api.Models;

public class GameResultResponse
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public long CompletionTimeMs { get; set; }
    public string CompletionTimeFormatted { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? Platform { get; set; }
}

