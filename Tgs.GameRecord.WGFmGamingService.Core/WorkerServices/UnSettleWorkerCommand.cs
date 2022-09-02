namespace Tgs.GameRecord.WGFmGamingService.Core.WorkerServices;

public record UnSettleWorkerCommand(
    Uri ApiUrl,
    string PartnerCode,
    string PartnerKey,
    string PartnerIV,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
