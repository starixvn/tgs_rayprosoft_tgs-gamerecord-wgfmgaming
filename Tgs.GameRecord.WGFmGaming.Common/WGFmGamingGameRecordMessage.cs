using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.WGFmGaming.Common;

public record WGFmGamingGameRecordMessage
{
	[Required]
	[DataType(DataType.Url)]
	[DefaultValue(typeof(Uri), "https://api.jsc080.net/")]
	public Uri ApiUrl { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "TA")]
	public string PartnerCode { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "abc1238888888888")]
	public string PartnerKey { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "cba1238888888888")]
	public string PartnerIV { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2024-08-19T00:00:00+08")]
	public DateTimeOffset StartTime { get; init; }
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2024-08-20T00:00:00+08")]
	public DateTimeOffset EndTime { get; init; }
}
