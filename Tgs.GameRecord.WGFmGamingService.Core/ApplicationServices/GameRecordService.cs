using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Tgs.GameProviders.Client;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices
{
    internal class GameRecordService : IGetService<GameRecordQuery, GameRecordResult>
    {
        private readonly IGameRecordListClient _client;
        private readonly GameRecordInfoOptions _gameRecordInfoOptions;
        private readonly ILogger<GameRecordService> _logger;
        private readonly IMessageSender _messageSender;
        private readonly string _subject;
        private readonly IUtensil _utensil;

        public GameRecordService(
            IOptions<GameRecordInfoOptions> options,
            ILogger<GameRecordService> logger,
            IGameRecordListClient client,
            IMessageSender messageSender,
            IUtensil utensil)
        {
            _gameRecordInfoOptions = options.Value;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
            _subject = $"tgs.gp.gamerecord.database.round.{_gameRecordInfoOptions.GameProviderCode.ToLower()}";
        }

        public async ValueTask<GameRecordResult> GetAsync(GameRecordQuery query, CancellationToken cancellationToken = default)
        {
            (bool Success, int Count, int SuccessCount, int FailCount, DateTimeOffset? LastGameRecordTime) responseUnSettle = await UnSettleAsync(query, cancellationToken).ConfigureAwait(false);
            if (!responseUnSettle.Success)
            {
                return new()
                {
                    Success = false
                };
            }

            (bool Success, int Count, int SuccessCount, int FailCount, DateTimeOffset? LastGameRecordTime) responseSettle = await SettleAsync(query, cancellationToken).ConfigureAwait(false);
            if (!responseSettle.Success)
            {
                return new()
                {
                    Success = false
                };
            }

            var LastGameRecorTime = responseUnSettle.LastGameRecordTime;
            if (LastGameRecorTime == null)
            {
                LastGameRecorTime = responseSettle.LastGameRecordTime;
            }
            else
            {
                if (responseSettle.LastGameRecordTime != null)
                {
                    LastGameRecorTime = (LastGameRecorTime < responseSettle.LastGameRecordTime) ? responseSettle.LastGameRecordTime : LastGameRecorTime;
                }
            }

            return new GameRecordResult()
            {
                Success = true,
                Count = responseUnSettle.Count + responseSettle.Count,
                SuccessCount = responseUnSettle.SuccessCount + responseSettle.SuccessCount,
                FailCount = responseUnSettle.FailCount + responseSettle.FailCount,
                LastGameRecorTime = LastGameRecorTime
            };
        }

        private async ValueTask<(bool Success, int Count, int SuccessCount, int FailCount, DateTimeOffset? LastGameRecordTime)> ExecuteRecordAsync(GameRecordModle[] datas, CancellationToken cancellationToken = default)
        {
            var count = 0;
            var successCount = 0;
            var failCount = 0;
            var lastGameRecordTime = default(DateTimeOffset?);

            foreach (var data in datas)
            {
                count++;

                var success = true;
                var gameProviderId = await _utensil.GetGameProviderIdAsync().ConfigureAwait(false);

                var playerModel = await _utensil.GetPlayerAsync(data.PlayerCode).ConfigureAwait(false);
                var gameId = await _utensil.GetGameIdAsync(gameProviderId, data.ExternalGameId).ConfigureAwait(false);
                if (playerModel == null || !gameId.HasValue)
                {
                    success = false;
                }

                Guid? gameProviderConfigurationId = default;
                if (playerModel != null)
                {
                    gameProviderConfigurationId = await _utensil.BrandGetGameProviderConfigurationIdAsync(playerModel.BrandId, gameProviderId).ConfigureAwait(false);
                    if (!gameProviderConfigurationId.HasValue)
                    {
                        success = false;
                    }
                }

                if (success)
                {
                    successCount++;
                }
                else
                {
                    failCount++;
                }

                var messageRequest = new GameRoundMessageRequest()
                {
                    GameProviderCode = data.GameProviderCode,
                    GameProviderId = data.GameProviderId,
                    ExternalRoundId = data.ExternalRoundId,
                    PlayerCode = (playerModel == null) ? string.Empty : playerModel.PlayerCode,
                    GameId = gameId ?? -1,
                    ExternalGameId = data.ExternalGameId,
                    CurrencyCode = (playerModel == null) ? string.Empty : playerModel.DefaultWalletCurrencyCode,
                    Status = data.Status,
                    GameProviderConfigurationId = gameProviderConfigurationId ?? Guid.Empty,
                    WalletId = (playerModel == null) ? Guid.Empty : playerModel.DefaultWalletId,
                    BrandId = (playerModel == null) ? Guid.Empty : playerModel.BrandId,
                    ExternalCreatedOn = data.ExternalCreatedOn,
                    ExternalClosedOn = data.ExternalClosedOn,
                    BetAmount = data.BetAmount,
                    ValidBetAmount = data.ValidBetAmount,
                    PlayerWinLoss = data.PlayerWinLoss,
                    Commission = data.Commission,
                    Turnover = data.Turnover,
                    SettleAmount = data.SettleAmount,
                    ClientStatus = data.ClientStatus,
                    AdditionalData = data.AdditionalData,
                    Success = success
                };

                _logger.LogInformation("LogOn:{logOn}  Subject:{subject} MessageRequest:{request}", DateTime.Now.ToString("o"), _subject, messageRequest);

                await _messageSender.PublishAsync(
                    _subject,
                    messageRequest.ToBson(),
                    cancellationToken).ConfigureAwait(false);

                lastGameRecordTime = data.ExternalClosedOn.HasValue ? data.ExternalClosedOn : data.ExternalCreatedOn;
            }

            return (true, count, successCount, failCount, lastGameRecordTime);
        }

        private async ValueTask<(bool Success, int Count, int SuccessCount, int FailCount, DateTimeOffset? LastGameRecordTime)> SettleAsync(GameRecordQuery source, CancellationToken cancellationToken = default)
        {
            var response = await _client.WGSportsGameRecordListAsync(
                true,
                source.StartTime,
                source.EndTime).ConfigureAwait(false);

            return response == null || !response.Success
                ? (false, 0, 0, 0, null)
                : response.Success && response?.Datas == null
                ? (true, 0, 0, 0, null)
                : await ExecuteRecordAsync(response?.Datas!, cancellationToken).ConfigureAwait(false);
        }

        private async ValueTask<(bool Success, int Count, int SuccessCount, int FailCount, DateTimeOffset? LastGameRecordTime)> UnSettleAsync(GameRecordQuery source, CancellationToken cancellationToken = default)
        {
            var response = await _client.WGSportsGameRecordListAsync(
                false,
                source.StartTime,
                source.EndTime
                ).ConfigureAwait(false);

            return response == null || !response.Success
                ? (false, 0, 0, 0, null)
                : response.Success && response?.Datas == null
                ? (true, 0, 0, 0, null)
                : await ExecuteRecordAsync(response?.Datas!, cancellationToken).ConfigureAwait(false);
        }
    }
}
