using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.WGFmGamingService.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;

namespace Tgs.GameRecord.WGFmGamingService.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "WGFM",
            GameProviderConfiguration = "WGFMConfiguration",
            IntervalMinutes = 3,
            IntervalMinutesMax = 1440,
            IntervalMinutesMinimum = 1,
            LockMinute = 2
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            Record: new GameHistoryInfo
            {
                ExternalRoundId = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                ExternalGameId = "1",
                PlayerCode = "4ucmqplxb029",
                IsClosed = true,
                IsVoid = false,
                ExternalCreatedOn = DateTimeOffset.Now.AddMonths(-30).ToString("O"),
                ExternalClosedOn = DateTimeOffset.Now.ToString("O"),
                Bet = 10m,
                ValidBet = 5m,
                Settle = 7.5m,
                Turnover = 5m,
                PlayerWinLoss = 2.5m,
                AdditionalData = "{}"
            });

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeClient);

        await service
            .ExecuteAsync(
            command: command)
            .ConfigureAwait(false);

        await fakeClient
           .Received(1)
           .MessageAsync(
           gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
           externalRoundId: Arg.Is(command.Record.ExternalRoundId),
           externalGameId: Arg.Is(command.Record.ExternalGameId),
           playerCode: Arg.Is(command.Record.PlayerCode),
           status: Arg.Is(GameRecordStatus.Closed),
           clientStatus: Arg.Is(GameRecordClientStatus.Win),
           externalCreatedOn: Arg.Is(DateTimeOffset.Parse(command.Record.ExternalCreatedOn)),
           externalClosedOn: Arg.Is((DateTimeOffset?)DateTimeOffset.Parse(command.Record.ExternalClosedOn)),
           bet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
           validBet: Arg.Is(command.Record.ValidBet.ToDecimal().GetValueOrDefault(0)),
           settle: Arg.Is(command.Record.Settle.ToDecimal().GetValueOrDefault(0)),
           commission: Arg.Is(0.0m),
           turnover: Arg.Is(command.Record.Turnover.ToDecimal().GetValueOrDefault(0)),
           playerWinLoss: Arg.Is(command.Record.PlayerWinLoss.ToDecimal().GetValueOrDefault(0)),
           additionalData: Arg.Is(command.Record.AdditionalData),
           cancellationToken: Arg.Any<CancellationToken>())
           .ConfigureAwait(false);
    }

    [Fact]
    public async void UnSettle()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "WGFM",
            GameProviderConfiguration = "WGFMConfiguration",
            IntervalMinutes = 3,
            IntervalMinutesMax = 1440,
            IntervalMinutesMinimum = 1,
            LockMinute = 2
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            Record: new GameHistoryInfo
            {
                ExternalRoundId = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                ExternalGameId = "1",
                PlayerCode = "4ucmqplxb029",
                IsClosed = false,
                IsVoid = false,
                ExternalCreatedOn = DateTimeOffset.Now.AddMonths(-30).ToString("O"),
                ExternalClosedOn = DateTimeOffset.UnixEpoch.ToString("O"),
                Bet = 10m,
                ValidBet = 5m,
                Settle = 0m,
                Turnover = 0m,
                PlayerWinLoss = 0m,
                AdditionalData = "{}"
            });

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeClient);

        await service
            .ExecuteAsync(
            command: command)
            .ConfigureAwait(false);

        await fakeClient
           .Received(1)
           .MessageAsync(
           gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
           externalRoundId: Arg.Is(command.Record.ExternalRoundId),
           externalGameId: Arg.Is(command.Record.ExternalGameId),
           playerCode: Arg.Is(command.Record.PlayerCode),
           status: Arg.Is(GameRecordStatus.Open),
           clientStatus: Arg.Is(GameRecordClientStatus.Open),
           externalCreatedOn: Arg.Is(DateTimeOffset.Parse(command.Record.ExternalCreatedOn)),
           externalClosedOn: Arg.Is((DateTimeOffset?)null),
           bet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
           validBet: Arg.Is(command.Record.ValidBet.ToDecimal().GetValueOrDefault(0)),
           settle: Arg.Is(command.Record.Settle.ToDecimal().GetValueOrDefault(0)),
           commission: Arg.Is(0.0m),
           turnover: Arg.Is(command.Record.Turnover.ToDecimal().GetValueOrDefault(0)),
           playerWinLoss: Arg.Is(command.Record.PlayerWinLoss.ToDecimal().GetValueOrDefault(0)),
           additionalData: Arg.Is(command.Record.AdditionalData),
           cancellationToken: Arg.Any<CancellationToken>())
           .ConfigureAwait(false);
    }

    [Fact]
    public async void Void()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "WGFM",
            GameProviderConfiguration = "WGFMConfiguration",
            IntervalMinutes = 3,
            IntervalMinutesMax = 1440,
            IntervalMinutesMinimum = 1,
            LockMinute = 2
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            Record: new GameHistoryInfo
            {
                ExternalRoundId = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                ExternalGameId = "1",
                PlayerCode = "4ucmqplxb029",
                IsClosed = true,
                IsVoid = true,
                ExternalCreatedOn = DateTimeOffset.Now.AddMonths(-30).ToString("O"),
                ExternalClosedOn = DateTimeOffset.Now.ToString("O"),
                Bet = 0m,
                ValidBet = 0m,
                Settle = 0m,
                Turnover = 0m,
                PlayerWinLoss = 0m,
                AdditionalData = "{}"
            });

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeClient);

        await service
            .ExecuteAsync(
            command: command)
            .ConfigureAwait(false);

        await fakeClient
           .Received(1)
           .MessageAsync(
           gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
           externalRoundId: Arg.Is(command.Record.ExternalRoundId),
           externalGameId: Arg.Is(command.Record.ExternalGameId),
           playerCode: Arg.Is(command.Record.PlayerCode),
           status: Arg.Is(GameRecordStatus.Closed),
           clientStatus: Arg.Is(GameRecordClientStatus.Void),
           externalCreatedOn: Arg.Is(DateTimeOffset.Parse(command.Record.ExternalCreatedOn)),
           externalClosedOn: Arg.Is((DateTimeOffset?)DateTimeOffset.Parse(command.Record.ExternalClosedOn)),
           bet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
           validBet: Arg.Is(command.Record.ValidBet.ToDecimal().GetValueOrDefault(0)),
           settle: Arg.Is(command.Record.Settle.ToDecimal().GetValueOrDefault(0)),
           commission: Arg.Is(0.0m),
           turnover: Arg.Is(command.Record.Turnover.ToDecimal().GetValueOrDefault(0)),
           playerWinLoss: Arg.Is(command.Record.PlayerWinLoss.ToDecimal().GetValueOrDefault(0)),
           additionalData: Arg.Is(command.Record.AdditionalData),
           cancellationToken: Arg.Any<CancellationToken>())
           .ConfigureAwait(false);
    }
}
