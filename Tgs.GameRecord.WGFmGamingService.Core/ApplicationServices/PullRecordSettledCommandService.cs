using Microsoft.Extensions.Logging;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameInterface.WGFmGamingService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;

internal class PullRecordSettledCommandService : ICommandService<PullRecordSettledCommand>
{
    private readonly IGameInterfaceWGFmGamingClient _client;
    private readonly ILogger<PullRecordSettledCommandService> _logger;
    private readonly ICommandService<HandleRecordCommand> _service;

    public PullRecordSettledCommandService(
        ILogger<PullRecordSettledCommandService> logger,
        IGameInterfaceWGFmGamingClient client,
        ICommandService<HandleRecordCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(PullRecordSettledCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var gameHistoryInfos = await _client
                    .GameHistoryAsync(
                        apiUrl: command.ApiUrl,
                        partnerCode: command.PartnerCode,
                        partnerKey: command.PartnerKey,
                        partnerIV: command.PartnerIV,
                        isSettle: true,
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
                    _logger.LogError(ex, "已結算遊戲紀錄發生例外.({gameHistory})", gameHistory);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"發生例外");
        }
    }
}
