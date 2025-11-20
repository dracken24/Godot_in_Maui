using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PlatformGame.Api.Models;

[Table("game_results")]
public class GameResult : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }
    
    [Column("player_name")]
    public string? PlayerName { get; set; }
    
    [Column("completion_time_ms")]
    public long CompletionTimeMs { get; set; }
    
    [Column("completion_time_formatted")]
    public string CompletionTimeFormatted { get; set; } = string.Empty;
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("platform")]
    public string? Platform { get; set; }
}

