namespace Tgs.GameRecord.WGFmGamingService.Core
{
    public record GameRecordInfoOptions
    {
        public string GameProviderCode { get; set; } = default!;
        public string GameProviderConfiguration { get; set; } = default!;
        public int IntervalMinuteMax { get; set; } = 60;
        public int IntervalMinute { get; set; } = 20;
    }
}
