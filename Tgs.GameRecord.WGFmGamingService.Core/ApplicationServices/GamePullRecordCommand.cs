using Tgs.GameInterface.WGFmGamingService.Protos;

namespace Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;

public record GamePullRecordCommand(IEnumerable<GameHistoryInfo> GameHistories);
