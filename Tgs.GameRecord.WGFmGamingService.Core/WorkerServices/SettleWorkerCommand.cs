namespace Tgs.GameRecord.WGFmGamingService.Core.WorkerServices;

public record SettleWorkerCommand(
    Uri ApiUrl,
    string PartnerCode,
    string PartnerKey,
    string partnerIV,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
