using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGaming.Core.ApplicationServices;

internal class HandleRecordCommandService(
    IOptions<GameRecordInfoOptions> gameRecordInfoOptions,
    IGameRecordHandleClient client) : ICommandService<HandleRecordCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo = gameRecordInfoOptions.Value;

    public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var wagerState = WagerStates.Running;
        var externalCreatedOn = DateTimeOffset.Parse(command.Record.ExternalCreatedOn);
        var externalClosedOn = DateTimeOffset.UnixEpoch;
        var settle = 0.0m;
        var commission = 0.0m;
        var turnover = 0.0m;
        var winLoss = 0.0m;
        if (command.Record.IsClosed)
        {
            externalClosedOn = DateTimeOffset.Parse(command.Record.ExternalClosedOn);
            settle = command.Record.Settle.ToDecimal().GetValueOrDefault(0);
            turnover = command.Record.Turnover.ToDecimal().GetValueOrDefault(0);
            winLoss = command.Record.PlayerWinLoss.ToDecimal().GetValueOrDefault(0);
            wagerState = command.Record.IsVoid
                ? WagerStates.Void
                : winLoss > 0
                ? WagerStates.Win
                : WagerStates.Lose;
        }

        await client
            .MessageAsync(
            gameProviderCode: _gameRecordInfo.GameProviderCode,
            playerCode: command.Record.PlayerCode,
            wagerState: wagerState,
            wagerType: WagerTypes.Normal,
            freeGameState: FreeGameStates.None,
            gambleState: GambleStates.None,
            cashoutState: CashoutStates.None,
            resettleState: ResettleStates.None,
            externalRoundId: command.Record.ExternalRoundId,
            externalWagerId: command.Record.ExternalRoundId,
            externalGameId: command.Record.ExternalGameId,
            externalCreatedOn: externalCreatedOn,
            externalClosedOn: externalClosedOn,
            externalPayoffOn: externalClosedOn,
            externalResettleOn: DateTimeOffset.UnixEpoch,
            bet: command.Record.Bet.ToDecimal().GetValueOrDefault(0),
            validBet: command.Record.ValidBet.ToDecimal().GetValueOrDefault(0),
            settle: settle,
            commission: commission,
            turnover: turnover,
            winLoss: winLoss,
            additionalData: JsonNode.Parse(command.Record.AdditionalData)!,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
