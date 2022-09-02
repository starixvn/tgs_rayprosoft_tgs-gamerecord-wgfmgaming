namespace Tgs.GameRecord.WGFmGamingService.Core.WorkerServices;

public record SettleWorkerCommand(
    Uri ApiUrl,
    string PartnerCode,
    string PartnerKey,
    string PartnerIV,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
