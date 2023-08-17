using Microsoft.Extensions.Options;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;

internal class HandleRecordCommandService : ICommandService<HandleRecordCommand>
{
    private readonly IGameRecordHandleClient _client;
    private readonly GameRecordInfoOptions _gameRecordInfo;

    public HandleRecordCommandService(
        IOptions<GameRecordInfoOptions> gameRecordInfoOptions,
        IGameRecordHandleClient client)
    {
        _gameRecordInfo = gameRecordInfoOptions.Value;
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var status = (command.Record.IsClosed) ? GameRecordStatus.Closed : GameRecordStatus.Open;
        var clientStatus = (!command.Record.IsClosed)
            ? GameRecordClientStatus.Open
            : (command.Record.IsVoid)
            ? GameRecordClientStatus.Void
            : (command.Record.PlayerWinLoss > 0)
            ? GameRecordClientStatus.Win
            : GameRecordClientStatus.Loss;
        var externalCreatedOn = DateTimeOffset.Parse(command.Record.ExternalCreatedOn);
        DateTimeOffset? externalClosedOn = command.Record.IsClosed ? DateTimeOffset.Parse(command.Record.ExternalClosedOn) : null;

        await _client
            .MessageAsync(
            gameProviderCode: _gameRecordInfo.GameProviderCode,
            externalRoundId: command.Record.ExternalRoundId,
            externalGameId: command.Record.ExternalGameId,
            playerCode: command.Record.PlayerCode,
            status: status,
            clientStatus: clientStatus,
            externalCreatedOn: externalCreatedOn,
            externalClosedOn: externalClosedOn,
            bet: command.Record.Bet.ToDecimal().GetValueOrDefault(0),
            validBet: command.Record.ValidBet.ToDecimal().GetValueOrDefault(0),
            settle: command.Record.Settle.ToDecimal().GetValueOrDefault(0),
            commission: 0,
            turnover: command.Record.Turnover.ToDecimal().GetValueOrDefault(0),
            playerWinLoss: command.Record.PlayerWinLoss.ToDecimal().GetValueOrDefault(0),
            additionalData: command.Record.AdditionalData,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
