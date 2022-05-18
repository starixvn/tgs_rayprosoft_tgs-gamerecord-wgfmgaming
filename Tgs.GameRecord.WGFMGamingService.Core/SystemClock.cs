namespace Tgs.GameRecord.WGFmGamingService.Core
{
    internal class SystemClock : ISystemClock
    {
        public DateTimeOffset CurrentTime => DateTimeOffset.UtcNow;
    }
}
