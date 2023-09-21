using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.WGFmGaming.ViewModels;

public record GameRecordQueryViewModel
{
    [Required]
    public Uri ApiUrl { get; init; } = default!;
    [Required]
    public string PartnerCode { get; init; } = default!;
    [Required]
    public string PartnerKey { get; init; } = default!;
    [Required]
    public string PartnerIV { get; init; } = default!;
    [Required]
    public DateTimeOffset StartTime { get; init; }
    [Required]
    public DateTimeOffset EndTime { get; init; }
}
