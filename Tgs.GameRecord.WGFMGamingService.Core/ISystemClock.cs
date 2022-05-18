namespace Tgs.GameRecord.WGFMGamingService.Core
{
    public interface ISystemClock
    {
        DateTimeOffset CurrentTime { get; }
    }
}
