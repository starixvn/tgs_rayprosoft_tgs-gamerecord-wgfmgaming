namespace Tgs.GameRecord.WGFmGamingService.Core.WorkerServices;

public record UnSettleWorkerCommand(
    Uri ApiUrl,
    string PartnerCode,
    string PartnerKey,
    string partnerIV,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
