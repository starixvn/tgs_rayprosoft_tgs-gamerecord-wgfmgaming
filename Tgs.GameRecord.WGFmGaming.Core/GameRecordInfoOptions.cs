namespace Tgs.GameRecord.WGFmGaming.Core;

public record GameRecordInfoOptions
{
    public string GameProviderCode { get; set; } = default!;
    public string GameProviderConfiguration { get; set; } = default!;
    public string GameProviderHangfire { get; set; } = default!;
    public double IntervalMinutes { get; set; } = 3;
    public double IntervalMinutesMax { get; set; } = 1440;
    public double IntervalMinutesMinimum { get; set; } = 1;
    public double LockMinute { get; set; } = 2;
}
