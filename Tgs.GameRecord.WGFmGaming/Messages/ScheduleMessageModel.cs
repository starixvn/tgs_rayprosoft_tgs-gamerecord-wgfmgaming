namespace Tgs.GameRecord.WGFmGaming.Messages;

public record ScheduleMessageModel
{
    public Uri ApiUrl { get; init; } = default!;
    public string PartnerCode { get; init; } = default!;
    public string PartnerKey { get; init; } = default!;
    public string PartnerIV { get; init; } = default!;
}
