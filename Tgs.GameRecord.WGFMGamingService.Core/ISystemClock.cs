namespace Tgs.GameRecord.WGFmGamingService.Core
{
    public interface ISystemClock
    {
        DateTimeOffset CurrentTime { get; }
    }
}
