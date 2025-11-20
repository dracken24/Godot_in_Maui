using Microsoft.AspNetCore.Mvc;
using PlatformGame.Api.Models;
using PlatformGame.Api.Services;

namespace PlatformGame.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameResultsController : ControllerBase
{
    private readonly SupabaseService _supabaseService;
    private readonly ILogger<GameResultsController> _logger;
    private readonly IWebHostEnvironment _environment;

    public GameResultsController(SupabaseService supabaseService, ILogger<GameResultsController> logger, IWebHostEnvironment environment)
    {
        _supabaseService = supabaseService;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Endpoint de test pour vérifier la connexion à Supabase
    /// </summary>
    [HttpGet("test")]
    public IActionResult TestConnection()
    {
        try
        {
            var client = _supabaseService.GetClient();
            return Ok(new { 
                status = "OK", 
                message = "Connexion à Supabase réussie",
                supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL") ?? "Non configuré"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur de connexion à Supabase");
            return StatusCode(500, new { 
                status = "ERROR",
                error = "Impossible de se connecter à Supabase",
                message = ex.Message,
                details = _environment.IsDevelopment() ? ex.ToString() : null
            });
        }
    }

    /// <summary>
    /// Récupère tous les résultats triés par temps croissant (meilleurs temps en premier)
    /// </summary>
    [HttpGet("results")]
    public async Task<ActionResult<IEnumerable<GameResultResponse>>> GetAllResults()
    {
        try
        {
            var client = _supabaseService.GetClient();
            var response = await client
                .From<GameResult>()
                .Order(x => x.CompletionTimeMs, Supabase.Postgrest.Constants.Ordering.Ascending)
                .Get();

            var results = response.Models.Select(r => new GameResultResponse
            {
                Id = r.Id,
                PlayerName = r.PlayerName,
                CompletionTimeMs = r.CompletionTimeMs,
                CompletionTimeFormatted = r.CompletionTimeFormatted,
                CreatedAt = r.CreatedAt,
                Platform = r.Platform
            }).ToList();

            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des résultats: {Message}", ex.Message);
            return StatusCode(500, new { 
                error = "Erreur lors de la récupération des résultats",
                message = ex.Message,
                details = _environment.IsDevelopment() ? ex.ToString() : null
            });
        }
    }

    /// <summary>
    /// Récupère le meilleur temps (record)
    /// </summary>
    [HttpGet("results/best")]
    public async Task<ActionResult<GameResultResponse>> GetBestResult()
    {
        try
        {
            var client = _supabaseService.GetClient();
            var response = await client
                .From<GameResult>()
                .Order(x => x.CompletionTimeMs, Supabase.Postgrest.Constants.Ordering.Ascending)
                .Limit(1)
                .Get();

            var bestResult = response.Models.FirstOrDefault();
            if (bestResult == null)
            {
                return NotFound("Aucun résultat trouvé");
            }

            var result = new GameResultResponse
            {
                Id = bestResult.Id,
                PlayerName = bestResult.PlayerName,
                CompletionTimeMs = bestResult.CompletionTimeMs,
                CompletionTimeFormatted = bestResult.CompletionTimeFormatted,
                CreatedAt = bestResult.CreatedAt,
                Platform = bestResult.Platform
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du meilleur résultat");
            return StatusCode(500, "Erreur lors de la récupération du meilleur résultat");
        }
    }

    /// <summary>
    /// Récupère les N meilleurs temps (ex: top 10)
    /// </summary>
    [HttpGet("results/top/{count}")]
    public async Task<ActionResult<IEnumerable<GameResultResponse>>> GetTopResults(int count)
    {
        if (count <= 0 || count > 100)
        {
            return BadRequest("Le nombre doit être entre 1 et 100");
        }

        try
        {
            var client = _supabaseService.GetClient();
            var response = await client
                .From<GameResult>()
                .Order(x => x.CompletionTimeMs, Supabase.Postgrest.Constants.Ordering.Ascending)
                .Limit(count)
                .Get();

            var results = response.Models.Select(r => new GameResultResponse
            {
                Id = r.Id,
                PlayerName = r.PlayerName,
                CompletionTimeMs = r.CompletionTimeMs,
                CompletionTimeFormatted = r.CompletionTimeFormatted,
                CreatedAt = r.CreatedAt,
                Platform = r.Platform
            }).ToList();

            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des meilleurs résultats");
            return StatusCode(500, "Erreur lors de la récupération des meilleurs résultats");
        }
    }

    /// <summary>
    /// Enregistre un nouveau temps de completion (appelé par le jeu Godot)
    /// </summary>
    [HttpPost("results")]
    public async Task<ActionResult<GameResultResponse>> CreateResult([FromBody] CreateGameResultRequest request)
    {
        if (request.CompletionTimeMs <= 0)
        {
            return BadRequest("Le temps de completion doit être supérieur à 0");
        }

        try
        {
            // Formater le temps (ex: convertir millisecondes en format "M:SS.mmm")
            var formattedTime = FormatTime(request.CompletionTimeMs);

            var gameResult = new GameResult
            {
                Id = Guid.NewGuid(),
                PlayerName = request.PlayerName,
                CompletionTimeMs = request.CompletionTimeMs,
                CompletionTimeFormatted = formattedTime,
                CreatedAt = DateTime.UtcNow,
                Platform = request.Platform
            };

            var client = _supabaseService.GetClient();
            var response = await client
                .From<GameResult>()
                .Insert(gameResult);

            var insertedResult = response.Models.FirstOrDefault();
            if (insertedResult == null)
            {
                return StatusCode(500, "Erreur lors de l'insertion du résultat");
            }

            var result = new GameResultResponse
            {
                Id = insertedResult.Id,
                PlayerName = insertedResult.PlayerName,
                CompletionTimeMs = insertedResult.CompletionTimeMs,
                CompletionTimeFormatted = insertedResult.CompletionTimeFormatted,
                CreatedAt = insertedResult.CreatedAt,
                Platform = insertedResult.Platform
            };

            return CreatedAtAction(nameof(GetAllResults), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création du résultat");
            return StatusCode(500, "Erreur lors de la création du résultat");
        }
    }

    /// <summary>
    /// Récupère les statistiques (moyenne, nombre de tentatives, meilleur temps, etc.)
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<GameStatsResponse>> GetStats()
    {
        try
        {
            var client = _supabaseService.GetClient();
            var allResults = await client
                .From<GameResult>()
                .Get();

            var results = allResults.Models.ToList();
            
            if (!results.Any())
            {
                return Ok(new GameStatsResponse
                {
                    BestTimeMs = 0,
                    BestTimeFormatted = "N/A",
                    AverageTimeMs = 0,
                    AverageTimeFormatted = "N/A",
                    TotalAttempts = 0,
                    BestResult = null
                });
            }

            var bestResult = results.OrderBy(r => r.CompletionTimeMs).First();
            var averageTimeMs = (long)results.Average(r => r.CompletionTimeMs);

            var stats = new GameStatsResponse
            {
                BestTimeMs = bestResult.CompletionTimeMs,
                BestTimeFormatted = bestResult.CompletionTimeFormatted,
                AverageTimeMs = averageTimeMs,
                AverageTimeFormatted = FormatTime(averageTimeMs),
                TotalAttempts = results.Count,
                BestResult = new GameResultResponse
                {
                    Id = bestResult.Id,
                    PlayerName = bestResult.PlayerName,
                    CompletionTimeMs = bestResult.CompletionTimeMs,
                    CompletionTimeFormatted = bestResult.CompletionTimeFormatted,
                    CreatedAt = bestResult.CreatedAt,
                    Platform = bestResult.Platform
                }
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des statistiques: {Message}", ex.Message);
            return StatusCode(500, new { 
                error = "Erreur lors de la récupération des statistiques",
                message = ex.Message,
                details = _environment.IsDevelopment() ? ex.ToString() : null
            });
        }
    }

    /// <summary>
    /// Formate le temps en millisecondes au format "M:SS.mmm" ou "SS.mmm"
    /// </summary>
    private static string FormatTime(long milliseconds)
    {
        var totalSeconds = milliseconds / 1000.0;
        var minutes = (int)(totalSeconds / 60);
        var seconds = (int)(totalSeconds % 60);
        var ms = milliseconds % 1000;

        if (minutes > 0)
        {
            return $"{minutes}:{seconds:D2}.{ms:D3}";
        }
        return $"{seconds}.{ms:D3}";
    }
}

