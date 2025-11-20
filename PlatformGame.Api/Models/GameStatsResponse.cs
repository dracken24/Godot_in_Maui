namespace PlatformGame.Api.Models;

public class GameStatsResponse
{
    public long BestTimeMs { get; set; }
    public string BestTimeFormatted { get; set; } = string.Empty;
    public long AverageTimeMs { get; set; }
    public string AverageTimeFormatted { get; set; } = string.Empty;
    public int TotalAttempts { get; set; }
    public GameResultResponse? BestResult { get; set; }
}

