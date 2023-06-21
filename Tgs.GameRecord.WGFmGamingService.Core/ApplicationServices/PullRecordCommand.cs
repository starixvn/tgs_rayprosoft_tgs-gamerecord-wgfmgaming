using Tgs.GameInterface.WGFmGamingService.Protos;

namespace Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;

public record PullRecordCommand(IEnumerable<GameHistoryInfo> GameHistories);
