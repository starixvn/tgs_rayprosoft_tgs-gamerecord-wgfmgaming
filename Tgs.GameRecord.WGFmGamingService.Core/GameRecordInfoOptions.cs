namespace Tgs.GameRecord.WGFmGamingService.Core;

public record GameRecordInfoOptions
{
    public string GameProviderCode { get; set; } = default!;
    public string GameProviderConfiguration { get; set; } = default!;
    public double MinimumIntervalMinute { get; set; } = 1;
    public double IntervalMinute { get; set; } = 3;
    public double LockMinute { get; set; } = 1.5;
}
