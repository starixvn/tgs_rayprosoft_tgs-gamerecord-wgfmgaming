using Microsoft.Extensions.Logging;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameInterface.WGFmGamingService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGaming.Core.ApplicationServices;

internal class PullRecordSettledCommandService(
    ILogger<PullRecordSettledCommandService> logger,
    IGameInterfaceWGFmGamingClient client,
    ICommandService<HandleRecordCommand> service) : ICommandService<PullRecordSettledCommand>
{
    public async ValueTask ExecuteAsync(PullRecordSettledCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var gameHistoryInfos = await client
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
                    await service
                    .ExecuteAsync(
                         command: new HandleRecordCommand(gameHistory),
                         cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game record.({gameRecord})", DateTime.Now.ToString("o"), gameHistory);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "LogOn:{logOn} Message:Exception occurred.({command})", DateTime.Now.ToString("o"), command);
        }
    }
}
