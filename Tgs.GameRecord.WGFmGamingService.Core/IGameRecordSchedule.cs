using Tgs.GameRecordSchedules.Protos;
using Tgs.GameRecord.WGFmGamingService.Core;

namespace Tgs.GameRecord.WGFmGamingService.Core
{
    public interface IGameRecordSchedule
    {
        ValueTask<GameRecordScheduleReply?> GetAsync(CancellationToken cancellationToken = default);

        ScheduleConfig GetConfig(GameRecordScheduleReply gameProviderSchedule);

        ValueTask<GameRecordScheduleReply?> InitAsync(Guid gameProviderId, CancellationToken cancellationToken = default);

        ValueTask<GameRecordScheduleReply?> UpdateAsync(GameRecordScheduleReply gameProviderSchedule, DateTimeOffset? lastGameRecorTime, DateTimeOffset sysTime, CancellationToken cancellationToken = default);
    }
}
