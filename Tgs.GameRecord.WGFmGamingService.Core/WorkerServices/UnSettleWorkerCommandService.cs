using Microsoft.Extensions.Logging;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGamingService.Core.WorkerServices;

internal class UnSettleWorkerCommandService : ICommandService<UnSettleWorkerCommand>
{
    private readonly IGameInterfaceWGFmGamingClient _client;
    private readonly ILogger<SettleWorkerCommandService> _logger;
    private readonly ICommandService<PullRecordCommand> _service;

    public UnSettleWorkerCommandService(
        ILogger<SettleWorkerCommandService> logger,
        IGameInterfaceWGFmGamingClient client,
        ICommandService<PullRecordCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(UnSettleWorkerCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var gameHistoryInfos = await _client
                    .GameHistoryAsync(
                        apiUrl: command.ApiUrl,
                        partnerCode: command.PartnerCode,
                        partnerKey: command.PartnerKey,
                        partnerIV: command.PartnerIV,
                        isSettle: false,
                        startTime: command.StartTime,
                        endTime: command.EndTime,
                        cancellationToken: cancellationToken)
                    .ToArrayAsync(cancellationToken)
                    .ConfigureAwait(false);

            if (!gameHistoryInfos.Any())
            {
                return;
            }

            var query = new PullRecordCommand(gameHistoryInfos);

            await _service
                .ExecuteAsync(
                    query,
                    cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"發生例外");
        }
    }
}
