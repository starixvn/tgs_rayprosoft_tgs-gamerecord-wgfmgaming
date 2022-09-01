using Microsoft.Extensions.Options;
using Tgs.GameRecord.WGFmGamingService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGamingService.Core.DispatchServices;

internal class ScheduleDispatchService : ICommandService<ScheduleDispatchCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfoOptions;
    private readonly double _intervalMinute;
    private readonly IScheduleDispatch _scheduleDispatch;
    private readonly ICommandService<SettleWorkerCommand> _settleService;
    private readonly ICommandService<UnSettleWorkerCommand> _unSettleService;
    private readonly IUtensil _utensil;

    public ScheduleDispatchService(
        IOptions<GameRecordInfoOptions> options,
        ICommandService<SettleWorkerCommand> settleService,
        ICommandService<UnSettleWorkerCommand> unSettleService,
        IScheduleDispatch scheduleDispatch,
        IUtensil utensil
        )
    {
        _gameRecordInfoOptions = options.Value;
        ;
        _settleService = settleService ?? throw new ArgumentNullException(nameof(settleService));
        _unSettleService = unSettleService ?? throw new ArgumentNullException(nameof(unSettleService));
        _scheduleDispatch = scheduleDispatch ?? throw new ArgumentNullException(nameof(scheduleDispatch));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        _intervalMinute = (_gameRecordInfoOptions.IntervalMinute < _gameRecordInfoOptions.MinimumIntervalMinute)
                        ? _gameRecordInfoOptions.MinimumIntervalMinute
                        : _gameRecordInfoOptions.IntervalMinute;
    }

    public async ValueTask ExecuteAsync(ScheduleDispatchCommand command, CancellationToken cancellationToken = default)
    {
        var configuration = await _utensil.GetSettingConfigurationAsync().ConfigureAwait(false);
        var key = $"gp:gamerecord:{_gameRecordInfoOptions.GameProviderCode}:{configuration.PartnerCode}".ToLower();
        if (!await _scheduleDispatch.ExistAsync(key: key).ConfigureAwait(false))
        {
            if (await _scheduleDispatch.LockingAsync(key: key, command: command).ConfigureAwait(false))
            {
                try
                {
                    var endDate = DateTimeOffset.ParseExact(DateTimeOffset.UtcNow.ToString(@"yyyy-MM-dd\THH:mm:00K"), @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
                    var startDate = endDate.AddMinutes(_intervalMinute * -1);

                    await _unSettleService.ExecuteAsync(new UnSettleWorkerCommand(
                        ApiUrl: configuration.ApiUrl,
                        PartnerCode: configuration.PartnerCode,
                        PartnerKey: configuration.PartnerKey,
                        partnerIV: configuration.IV,
                        StartTime: startDate,
                        EndTime: endDate), cancellationToken: cancellationToken).ConfigureAwait(false);

                    await _settleService.ExecuteAsync(new SettleWorkerCommand(
                        ApiUrl: configuration.ApiUrl,
                        PartnerCode: configuration.PartnerCode,
                        PartnerKey: configuration.PartnerKey,
                        partnerIV: configuration.IV,
                        StartTime: startDate,
                        EndTime: endDate), cancellationToken: cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    await _scheduleDispatch.UnlockAsync(key: key).ConfigureAwait(false);
                }
            }
        }
    }
}
