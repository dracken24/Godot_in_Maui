namespace PlatformGame.Services;

public interface IGameApiService
{
    Task<List<GameResultResponse>> GetAllResultsAsync();
    Task<GameResultResponse?> GetBestResultAsync();
    Task<List<GameResultResponse>> GetTopResultsAsync(int count);
    Task<GameResultResponse?> CreateResultAsync(CreateGameResultRequest request);
    Task<GameStatsResponse?> GetStatsAsync();
}


