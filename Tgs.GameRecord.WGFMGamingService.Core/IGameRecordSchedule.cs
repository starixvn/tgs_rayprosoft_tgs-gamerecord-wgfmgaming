using Tgs.GameRecordSchedules.Protos;
using Tgs.GameRecord.WGFMGamingService.Core;

namespace Tgs.GameRecord.WGFMGamingService.Core
{
    public interface IGameRecordSchedule
    {
        ValueTask<GameRecordScheduleReply?> GetAsync(CancellationToken cancellationToken = default);

        ScheduleConfig GetConfig(GameRecordScheduleReply gameProviderSchedule);

        ValueTask<GameRecordScheduleReply?> InitAsync(Guid gameProviderId, CancellationToken cancellationToken = default);

        ValueTask<GameRecordScheduleReply?> UpdateAsync(GameRecordScheduleReply gameProviderSchedule, DateTimeOffset? lastGameRecorTime, DateTimeOffset sysTime, CancellationToken cancellationToken = default);
    }
}
