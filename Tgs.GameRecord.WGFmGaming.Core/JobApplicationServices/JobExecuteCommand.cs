namespace Tgs.GameRecord.WGFmGaming.Core.JobApplicationServices;

public record JobExecuteCommand(
    Uri ApiUrl,
    string PartnerCode,
    string PartnerKey,
    string PartnerIV);
