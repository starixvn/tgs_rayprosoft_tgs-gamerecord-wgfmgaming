namespace Tgs.GameRecord.WGFmGamingService.Core;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
