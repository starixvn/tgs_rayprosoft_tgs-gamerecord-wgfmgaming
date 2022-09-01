using Tgs.GameInterface.WGFmGamingService.Protos;

namespace Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;

public record GamePullRecordCommand(
    Guid GameProviderId,
    string GameProviderCode,
    IEnumerable<GameHistoryInfo> GameHistories);
