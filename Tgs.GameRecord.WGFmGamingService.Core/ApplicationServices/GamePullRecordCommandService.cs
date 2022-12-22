using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;

internal class GamePullRecordCommandService : ICommandService<GamePullRecordCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfoOptions;
    private readonly ILogger<GamePullRecordCommandService> _logger;
    private readonly IMessageSender _messageSender;
    private readonly string _subject;
    private readonly IUtensil _utensil;

    public GamePullRecordCommandService(
        IOptions<GameRecordInfoOptions> options,
        ILogger<GamePullRecordCommandService> logger,
        IMessageSender messageSender,
        IUtensil utensil)
    {
        _gameRecordInfoOptions = options.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        _subject = $"tgs.gp.gamerecord.database.round.{_gameRecordInfoOptions.GameProviderCode.ToLower()}";
    }

    public async ValueTask ExecuteAsync(GamePullRecordCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var gameHistory in command.GameHistories)
        {
            bool success = true;
            var playerModel = await _utensil.GetPlayerAsync(gameHistory.PlayerCode).ConfigureAwait(false);
            var gameId = await _utensil.GetGameIdAsync(command.GameProviderId, gameHistory.ExternalGameId).ConfigureAwait(false);
            if (playerModel == null || !gameId.HasValue)
            {
                success = false;
            }

            Guid? gameProviderConfigurationId = default;
            if (playerModel != null)
            {
                gameProviderConfigurationId = await _utensil.BrandGetGameProviderConfigurationIdAsync(playerModel.BrandId, command.GameProviderId).ConfigureAwait(false);
                if (!gameProviderConfigurationId.HasValue)
                {
                    success = false;
                }
            }

            var status = (gameHistory.IsClosed) ? GameRoundStatus.Closed : GameRoundStatus.Open;
            var clientStatus = (!gameHistory.IsClosed)
                ? GameRoundClientStatus.Open
                : (gameHistory.IsVoid)
                ? GameRoundClientStatus.Void : (gameHistory.PlayerWinLoss > 0)
                ? GameRoundClientStatus.Win
                : GameRoundClientStatus.Loss;
            var externalCreatedOn = DateTimeOffset.Parse(gameHistory.ExternalCreatedOn);
            DateTimeOffset? externalClosedOn = gameHistory.IsClosed ? DateTimeOffset.Parse(gameHistory.ExternalClosedOn) : null;

            var messageRequest = new GameRoundData(
                ExternalRoundId: gameHistory.ExternalRoundId,
                GameProviderId: command.GameProviderId,
                GameProviderCode: command.GameProviderCode,
                PlayerCode: (playerModel == null) ? string.Empty : playerModel.PlayerCode,
                GameId: gameId ?? -1,
                ExternalGameId: gameHistory.ExternalGameId,
                CurrencyCode: (playerModel == null) ? string.Empty : playerModel.DefaultWalletCurrencyCode,
                Status: status,
                GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
                WalletId: (playerModel == null) ? Guid.Empty : playerModel.DefaultWalletId,
                ExternalCreatedOn: externalCreatedOn,
                ExternalClosedOn: externalClosedOn,
                BrandId: (playerModel == null) ? Guid.Empty : playerModel.BrandId,
                BetAmount: gameHistory.Bet.ToDecimal(),
                ValidBetAmount: gameHistory.ValidBet.ToDecimal(),
                SettleAmount: gameHistory.Settle.ToDecimal(),
                Commission: 0,
                Turnover: gameHistory.Turnover.ToDecimal(),
                PlayerWinLoss: gameHistory.PlayerWinLoss.ToDecimal(),
                AdditionalData: gameHistory.AdditionalData,
                ClientStatus: clientStatus,
                Success: success
                );

            _logger.LogInformation("LogOn:{logOn}  Subject:{subject} MessageRequest:{request}", DateTime.Now.ToString("o"), _subject, messageRequest);

            await _messageSender.PublishAsync(
                _subject,
                messageRequest.ToBson(),
                cancellationToken).ConfigureAwait(false);
        }
    }
}
