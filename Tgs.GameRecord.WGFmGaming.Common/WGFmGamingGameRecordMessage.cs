using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.WGFmGaming.Common;

public record WGFmGamingGameRecordMessage
{
    [Required]
    [DataType(DataType.Url)]
    public Uri ApiUrl { get; init; } = default!;
    [Required]
    public string PartnerCode { get; init; } = default!;
    [Required]
    public string PartnerKey { get; init; } = default!;
    [Required]
    public string PartnerIV { get; init; } = default!;
    [Required]
    [DataType(DataType.DateTime)]
    public DateTimeOffset StartTime { get; init; }
    [Required]
    [DataType(DataType.DateTime)]
    public DateTimeOffset EndTime { get; init; }
}
