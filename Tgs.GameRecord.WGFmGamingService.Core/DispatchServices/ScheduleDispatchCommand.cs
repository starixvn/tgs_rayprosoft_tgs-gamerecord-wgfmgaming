namespace Tgs.GameRecord.WGFmGamingService.Core.DispatchServices;

public record ScheduleDispatchCommand(
    Uri ApiUrl,
    string PartnerCode,
    string PartnerKey,
    string PartnerIV);
