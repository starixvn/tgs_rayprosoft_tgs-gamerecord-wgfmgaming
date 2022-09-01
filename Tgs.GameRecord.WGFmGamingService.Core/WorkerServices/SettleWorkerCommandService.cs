using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGamingService.Core.WorkerServices;

internal class SettleWorkerCommandService : ICommandService<SettleWorkerCommand>
{
    private readonly IGameInterfaceWGFmGamingClient _client;
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly ILogger<SettleWorkerCommandService> _logger;
    private readonly ICommandService<GamePullRecordCommand> _service;
    private readonly IUtensil _utensil;

    public SettleWorkerCommandService(
        IOptions<GameRecordInfoOptions> options,
        ILogger<SettleWorkerCommandService> logger,
        IUtensil utensil,
        IGameInterfaceWGFmGamingClient client,
        ICommandService<GamePullRecordCommand> service)
    {
        _gameRecordInfo = options.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(SettleWorkerCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil.GetGameProviderIdAsync().ConfigureAwait(false);
        try
        {
            var gameHistoryInfos = await _client
                    .GameHistoryAsync(
                        apiUrl: command.ApiUrl,
                        partnerCode: command.PartnerCode,
                        partnerKey: command.PartnerKey,
                        partnerIV: command.partnerIV,
                        isSettle: true,
                        startTime: command.StartTime,
                        endTime: command.EndTime,
                        cancellationToken: cancellationToken)
                    .ToArrayAsync(cancellationToken)
                    .ConfigureAwait(false);

            if (!gameHistoryInfos.Any())
            {
                return;
            }

            var query = new GamePullRecordCommand(
                gameProviderId,
                _gameRecordInfo.GameProviderCode,
                gameHistoryInfos);

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
