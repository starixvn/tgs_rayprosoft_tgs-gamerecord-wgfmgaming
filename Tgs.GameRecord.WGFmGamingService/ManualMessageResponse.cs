namespace Tgs.GameRecord.WGFmGamingService
{
    public record ManualMessageResponse
    {
        public bool Success { get; init; }
        public DateTimeOffset? LastGameRecorTime { get; init; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public int Count { get; init; }
    }
}
