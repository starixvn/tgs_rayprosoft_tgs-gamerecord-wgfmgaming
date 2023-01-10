using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.MessageModels;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;

internal class GamePullRecordCommandService : ICommandService<GamePullRecordCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly ILogger<GamePullRecordCommandService> _logger;
    private readonly IMessageSender _messageSender;
    private readonly MongoSettingOptions _mongoSetting;
    private readonly IUtensil _utensil;

    public GamePullRecordCommandService(
        ILogger<GamePullRecordCommandService> logger,
        IOptions<GameRecordInfoOptions> gameRecordInfoOptions,
        IOptions<MongoSettingOptions> mongoSettingOptions,
        IMessageSender messageSender,
        IUtensil utensil)
    {
        _gameRecordInfo = gameRecordInfoOptions.Value;
        _mongoSetting = mongoSettingOptions.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask ExecuteAsync(GamePullRecordCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil
            .GetGameProviderIdAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        foreach (var gameHistory in command.GameHistories)
        {
            bool success = true;
            var playerModel = await _utensil
                .GetPlayerAsync(
                    playerCode: gameHistory.PlayerCode,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            var gameId = await _utensil
                .GetGameIdAsync(
                    gameProviderId: gameProviderId,
                    gameExternalId: gameHistory.ExternalGameId,
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
            var status = (gameHistory.IsClosed) ? GameRoundStatus.Closed : GameRoundStatus.Open;
            var clientStatus = (!gameHistory.IsClosed)
                ? GameRoundClientStatus.Open
                : (gameHistory.IsVoid)
                ? GameRoundClientStatus.Void
                : (gameHistory.PlayerWinLoss > 0)
                ? GameRoundClientStatus.Win
                : GameRoundClientStatus.Loss;
            var externalCreatedOn = DateTimeOffset.Parse(gameHistory.ExternalCreatedOn);
            DateTimeOffset? externalClosedOn = gameHistory.IsClosed ? DateTimeOffset.Parse(gameHistory.ExternalClosedOn) : null;

            var message = new GameRecordMessage(
                ExternalRoundId: gameHistory.ExternalRoundId,
                GameProviderId: gameProviderId,
                GameProviderCode: _gameRecordInfo.GameProviderCode,
                GameId: gameId ?? -1,
                ExternalGameId: gameHistory.ExternalGameId,
                PlayerCode: (playerModel == null) ? string.Empty : playerModel.PlayerCode,
                CurrencyCode: (playerModel == null) ? string.Empty : playerModel.DefaultWalletCurrencyCode,
                WalletId: (playerModel == null) ? Guid.Empty : playerModel.DefaultWalletId,
                BrandId: (playerModel == null) ? Guid.Empty : playerModel.BrandId,
                GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
                Status: status,
                ClientStatus: clientStatus,
                ExternalCreatedOn: externalCreatedOn,
                ExternalClosedOn: externalClosedOn,
                BetAmount: gameHistory.Bet.ToDecimal().GetValueOrDefault(0),
                ValidBetAmount: gameHistory.ValidBet.ToDecimal().GetValueOrDefault(0),
                SettleAmount: gameHistory.Settle.ToDecimal().GetValueOrDefault(0),
                Commission: 0,
                Turnover: gameHistory.Turnover.ToDecimal().GetValueOrDefault(0),
                PlayerWinLoss: gameHistory.PlayerWinLoss.ToDecimal().GetValueOrDefault(0),
                AdditionalData: gameHistory.AdditionalData,
                Success: success);

            _logger.LogInformation("LogOn:{logOn}  Subject:{subject} MessageRequest:{message}",
                DateTime.Now.ToString("o"),
                _mongoSetting.GlobPattern,
                message);

            await _messageSender
                .PublishAsync(
                    subject: _mongoSetting.GlobPattern,
                    data: message.ToBson(),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
