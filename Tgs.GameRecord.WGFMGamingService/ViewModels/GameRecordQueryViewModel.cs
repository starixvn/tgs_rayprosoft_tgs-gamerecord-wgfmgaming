namespace Tgs.GameRecord.WGFMGamingService.ViewModels
{
    public record GameRecordQueryViewModel
    {
        public DateTimeOffset StartTime { get; init; } = default!;

        public DateTimeOffset EndTime { get; init; } = default!;
    }
}
