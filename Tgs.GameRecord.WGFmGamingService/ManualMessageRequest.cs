namespace Tgs.GameRecord.WGFmGamingService
{
    public record ManualMessageRequest
    {
        public DateTimeOffset StartTime { get; init; } = default!;

        public DateTimeOffset EndTime { get; init; } = default!;
    }
}
