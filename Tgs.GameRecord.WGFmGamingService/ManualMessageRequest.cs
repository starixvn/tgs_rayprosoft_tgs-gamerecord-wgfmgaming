namespace Tgs.GameRecord.WGFmGamingService;

public record ManualMessageRequest
{
    public Uri ApiUrl { get; init; } = default!;
    public string PartnerCode { get; init; } = default!;
    public string PartnerKey { get; init; } = default!;
    public string PartnerIV { get; init; } = default!;
    public DateTimeOffset StartTime { get; init; }
    public DateTimeOffset EndTime { get; init; }
}
