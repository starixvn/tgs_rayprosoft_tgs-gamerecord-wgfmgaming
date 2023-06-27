using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;

internal class PullRecordCommandService : ICommandService<PullRecordCommand>
{
    private readonly ILogger<PullRecordCommandService> _logger;
    private readonly ICommandService<SettlePullRecordCommand> _settleService;
    private readonly ICommandService<UnSettlePullRecordCommand> _unSettleService;

    public PullRecordCommandService(
        ILogger<PullRecordCommandService> logger,
        ICommandService<UnSettlePullRecordCommand> unSettleService,
        ICommandService<SettlePullRecordCommand> settleService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _unSettleService = unSettleService ?? throw new ArgumentNullException(nameof(unSettleService));
        _settleService = settleService ?? throw new ArgumentNullException(nameof(settleService));
    }

    public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var pullRecordCommand = new UnSettlePullRecordCommand(
                    ApiUrl: command.ApiUrl,
                    PartnerCode: command.PartnerCode,
                    PartnerKey: command.PartnerKey,
                    PartnerIV: command.PartnerIV,
                    StartTime: command.StartTime,
                    EndTime: command.EndTime);
            await _unSettleService
                .ExecuteAsync(
                command: pullRecordCommand,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得未結算紀錄發生例外.");
        }

        try
        {
            var pullRecordCommand = new SettlePullRecordCommand(
                    ApiUrl: command.ApiUrl,
                    PartnerCode: command.PartnerCode,
                    PartnerKey: command.PartnerKey,
                    PartnerIV: command.PartnerIV,
                    StartTime: command.StartTime,
                    EndTime: command.EndTime);
            await _settleService
                .ExecuteAsync(
                command: pullRecordCommand,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得已結算紀錄發生例外.");
        }
    }
}
