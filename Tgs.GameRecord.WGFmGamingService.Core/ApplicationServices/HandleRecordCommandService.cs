using System.Text.Json;
using Tgs.GameRounds.Work.Client;
using Tgs.GameRounds.Work.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;

internal class HandleRecordCommandService : ICommandService<HandleRecordCommand>
{
    private readonly IGameRoundsWorkClient _client;
    private readonly IUtensil _utensil;

    public HandleRecordCommandService(
        IGameRoundsWorkClient client,
        IUtensil utensil)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var success = true;
        var gameProviderId = await _utensil
            .GetGameProviderIdAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameProviderCode = _utensil.GetGameProviderCode();
        var playerModel = await _utensil
            .GetPlayerAsync(
                playerCode: command.Record.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameId = await _utensil
            .GetGameIdAsync(
                gameProviderId: gameProviderId,
                gameExternalId: command.Record.ExternalGameId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        if (playerModel == null || !gameId.HasValue)
        {
            success = false;
        }

        Guid? gameProviderConfigurationId = default;
        if (playerModel != null)
        {
            gameProviderConfigurationId = await _utensil
                .GetBrandGameProviderConfigurationIdAsync(
                    brandId: playerModel.BrandId,
                    gameProviderId: gameProviderId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            if (!gameProviderConfigurationId.HasValue)
            {
                success = false;
            }
        }
        if (success)
        {
            var status = (command.Record.IsClosed) ? RoundStatus.Closed : RoundStatus.Open;
            var clientStatus = (!command.Record.IsClosed)
                ? RoundClientStatus.Open
                : (command.Record.IsVoid)
                ? RoundClientStatus.Void
                : (command.Record.PlayerWinLoss > 0)
                ? RoundClientStatus.Win
                : RoundClientStatus.Loss;
            var externalCreatedOn = DateTimeOffset.Parse(command.Record.ExternalCreatedOn);
            DateTimeOffset? externalClosedOn = command.Record.IsClosed ? DateTimeOffset.Parse(command.Record.ExternalClosedOn) : null;

            await _client
            .RoundBidding(
                gameProviderId: gameProviderId,
                gameProviderCode: gameProviderCode,
                externalRoundId: command.Record.ExternalRoundId,
                gameId: gameId ?? -1,
                externalGameId: command.Record.ExternalGameId,
                playerCode: command.Record.PlayerCode,
                walletId: playerModel!.DefaultWalletId,
                currencyCode: playerModel!.DefaultWalletCurrencyCode,
                brandId: playerModel!.BrandId,
                gameProviderConfigurationId: gameProviderConfigurationId!.Value,
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
        else
        {
            await _client
            .RoundMissing(
                gameProviderId: gameProviderId,
                gameProviderCode: gameProviderCode,
                externalRoundId: command.Record.ExternalRoundId,
                gameId: gameId ?? -1,
                externalGameId: command.Record.ExternalGameId,
                playerCode: command.Record.PlayerCode,
                data: JsonSerializer.Serialize(command.Record),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        }
    }
}
