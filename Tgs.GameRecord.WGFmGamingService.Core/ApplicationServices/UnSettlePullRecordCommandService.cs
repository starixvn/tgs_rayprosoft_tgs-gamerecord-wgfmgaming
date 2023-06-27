using Microsoft.Extensions.Logging;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameInterface.WGFmGamingService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;

internal class UnSettlePullRecordCommandService : ICommandService<UnSettlePullRecordCommand>
{
    private readonly IGameInterfaceWGFmGamingClient _client;
    private readonly ILogger<SettlePullRecordCommandService> _logger;
    private readonly ICommandService<HandleRecordCommand> _service;

    public UnSettlePullRecordCommandService(
        ILogger<SettlePullRecordCommandService> logger,
        IGameInterfaceWGFmGamingClient client,
        ICommandService<HandleRecordCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(UnSettlePullRecordCommand command, CancellationToken cancellationToken = default)
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
                    .ConfigureAwait(false)
                    ?? Enumerable.Empty<GameHistoryInfo>();

            if (!gameHistoryInfos.Any())
                return;

            foreach (var gameHistory in gameHistoryInfos)
            {
                try
                {
                    await _service
                    .ExecuteAsync(
                         command: new HandleRecordCommand(gameHistory),
                         cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "未結算遊戲紀錄發生例外.({gameHistory})", gameHistory);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"發生例外");
        }
    }
}
