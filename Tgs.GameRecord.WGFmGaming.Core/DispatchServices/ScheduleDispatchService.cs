using Microsoft.Extensions.Options;
using Tgs.GameRecord.WGFmGaming.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGaming.Core.DispatchServices;

internal class ScheduleDispatchService : ICommandService<ScheduleDispatchCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfoOptions;
    private readonly double _intervalMinute;
    private readonly IScheduleDispatch _scheduleDispatch;
    private readonly ICommandService<PullRecordCommand> _service;

    public ScheduleDispatchService(
        IOptions<GameRecordInfoOptions> options,
        IScheduleDispatch scheduleDispatch,
        ICommandService<PullRecordCommand> service
        )
    {
        _gameRecordInfoOptions = options.Value;
        _scheduleDispatch = scheduleDispatch ?? throw new ArgumentNullException(nameof(scheduleDispatch));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _intervalMinute = (_gameRecordInfoOptions.IntervalMinutes < _gameRecordInfoOptions.IntervalMinutesMinimum)
                        ? _gameRecordInfoOptions.IntervalMinutesMinimum
                        : _gameRecordInfoOptions.IntervalMinutes;
    }

    public async ValueTask ExecuteAsync(ScheduleDispatchCommand command, CancellationToken cancellationToken = default)
    {
        var key = $"gp:gamerecord:{_gameRecordInfoOptions.GameProviderCode}:{command.PartnerCode}".ToLower();
        if (!await _scheduleDispatch.ExistAsync(key: key).ConfigureAwait(false))
        {
            if (await _scheduleDispatch.LockingAsync(key: key, command: command).ConfigureAwait(false))
            {
                try
                {
                    var endTime = DateTimeOffset.ParseExact(DateTimeOffset.UtcNow.ToString(@"yyyy-MM-dd\THH:mm:00K"), @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
                    var startTime = endTime.AddMinutes(_intervalMinute * -1);

                    var pullRecordCommand = new PullRecordCommand(
                            ApiUrl: command.ApiUrl,
                            PartnerCode: command.PartnerCode,
                            PartnerKey: command.PartnerKey,
                            PartnerIV: command.PartnerIV,
                            StartTime: startTime,
                            EndTime: endTime);
                    await _service
                        .ExecuteAsync(
                        command: pullRecordCommand,
                        cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                finally
                {
                    await _scheduleDispatch.UnlockAsync(key: key).ConfigureAwait(false);
                }
            }
        }
    }
}
