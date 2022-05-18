namespace Tgs.GameRecord.WGFMGamingService.Core
{
    internal class SystemClock : ISystemClock
    {
        public DateTimeOffset CurrentTime => DateTimeOffset.UtcNow;
    }
}
