namespace Tgs.GameRecord.WGFmGamingService.Core;

public record WGSportsConfiguration
{
    public Uri ApiUrl { get; init; } = default!;

    public string PartnerCode { get; init; } = default!;

    public string PartnerKey { get; init; } = default!;

    public string IV { get; init; } = default!;
}
