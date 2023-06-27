namespace Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;

public record SettlePullRecordCommand(
    Uri ApiUrl,
    string PartnerCode,
    string PartnerKey,
    string PartnerIV,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
