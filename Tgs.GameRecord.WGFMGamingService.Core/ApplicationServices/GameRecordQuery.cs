namespace Tgs.GameRecord.WGFMGamingService.Core.ApplicationServices
{
    public record GameRecordQuery
    {
        public DateTimeOffset StartTime { get; init; } = default!;

        public DateTimeOffset EndTime { get; init; } = default!;
    }
}
