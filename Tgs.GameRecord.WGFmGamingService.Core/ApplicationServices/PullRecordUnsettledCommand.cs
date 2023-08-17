namespace Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;

public record PullRecordUnsettledCommand(
    Uri ApiUrl,
    string PartnerCode,
    string PartnerKey,
    string PartnerIV,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
