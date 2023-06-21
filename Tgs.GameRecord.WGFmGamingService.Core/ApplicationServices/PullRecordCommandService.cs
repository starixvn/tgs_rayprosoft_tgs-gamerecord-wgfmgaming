using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;

internal class PullRecordCommandService : ICommandService<PullRecordCommand>
{
    private readonly ILogger<PullRecordCommandService> _logger;
    private readonly ICommandService<HandleRecordCommand> _service;

    public PullRecordCommandService(
        ILogger<PullRecordCommandService> logger,
        ICommandService<HandleRecordCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var gameHistory in command.GameHistories)
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
                _logger.LogError(ex, "遊戲紀錄發生例外.({gameHistory})", gameHistory);
            }
        }
    }
}
