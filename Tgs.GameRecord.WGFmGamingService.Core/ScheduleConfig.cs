namespace Tgs.GameRecord.WGFmGamingService.Core
{
    public record ScheduleConfig
    {
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public DateTimeOffset LastGameRecordTime { get; set; }
    }
}
