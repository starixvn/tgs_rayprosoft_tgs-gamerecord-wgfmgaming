namespace Tgs.GameRecord.WGFmGaming.Core.ApplicationServices;

public record PullRecordCommand(
	Uri ApiUrl,
	string PartnerCode,
	string PartnerKey,
	string PartnerIV,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
