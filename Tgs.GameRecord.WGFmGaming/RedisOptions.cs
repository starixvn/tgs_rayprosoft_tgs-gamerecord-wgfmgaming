using Tgs.GameRecord.WGFmGaming.Core;
namespace Tgs.GameRecord.WGFmGaming;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
