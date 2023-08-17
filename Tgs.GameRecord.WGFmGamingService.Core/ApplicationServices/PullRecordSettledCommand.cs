namespace Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;

public record PullRecordSettledCommand(
    Uri ApiUrl,
    string PartnerCode,
    string PartnerKey,
    string PartnerIV,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
