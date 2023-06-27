using System.Text.Json;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.WGFmGamingService.Protos;
using Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;
using Tgs.GameRounds.Work.Client;
using Tgs.GameRounds.Work.Common.Protos;

namespace Tgs.GameRecord.WGFmGamingService.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        var fakeGameRoundsWorkClient = Substitute.For<IGameRoundsWorkClient>();
        var fakeUtensil = Substitute.For<IUtensil>();

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

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "WGFM";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakeUtensil
            .GetPlayerAsync(
            playerCode: Arg.Is(command.Record.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        int? gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameExternalId: Arg.Is(command.Record.ExternalGameId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        Guid? gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var service = new HandleRecordCommandService(
            fakeUtensil,
            fakeGameRoundsWorkClient);

        await service
            .ExecuteAsync(
            command: command)
            .ConfigureAwait(false);

        await fakeGameRoundsWorkClient
           .Received(1)
           .RoundBidding(
           gameProviderId: Arg.Is(gameProviderId),
           gameProviderCode: Arg.Is(gameProviderCode),
           externalRoundId: Arg.Is(command.Record.ExternalRoundId),
           gameId: Arg.Is(gameId ?? -1),
           externalGameId: Arg.Is(command.Record.ExternalGameId),
           playerCode: Arg.Is(command.Record.PlayerCode),
           walletId: Arg.Is(player!.DefaultWalletId),
           currencyCode: Arg.Is(player!.DefaultWalletCurrencyCode),
           brandId: Arg.Is(player!.BrandId),
           gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId!.Value),
           status: Arg.Is(RoundStatus.Closed),
           clientStatus: Arg.Is(RoundClientStatus.Win),
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

        await fakeGameRoundsWorkClient
            .Received(0)
            .RoundMissing(
            gameProviderId: Arg.Is(gameProviderId),
            gameProviderCode: Arg.Is(gameProviderCode),
            externalRoundId: Arg.Is(command.Record.ExternalRoundId),
            gameId: Arg.Is(gameId ?? -1),
            externalGameId: Arg.Is(command.Record.ExternalGameId),
            playerCode: Arg.Is(command.Record.PlayerCode),
            data: Arg.Is(JsonSerializer.Serialize(command.Record)),
            cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Fact]
    public async void Round_Missing()
    {
        var fakeGameRoundsWorkClient = Substitute.For<IGameRoundsWorkClient>();
        var fakeUtensil = Substitute.For<IUtensil>();

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

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "WGFM";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakeUtensil
            .GetPlayerAsync(
            playerCode: Arg.Is(command.Record.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        int? gameId = null;
        _ = fakeUtensil
            .GetGameIdAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameExternalId: Arg.Is(command.Record.ExternalGameId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        Guid? gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var service = new HandleRecordCommandService(
            fakeUtensil,
            fakeGameRoundsWorkClient);

        await service
            .ExecuteAsync(
            command: command)
            .ConfigureAwait(false);

        await fakeGameRoundsWorkClient
           .Received(0)
           .RoundBidding(
           gameProviderId: Arg.Is(gameProviderId),
           gameProviderCode: Arg.Is(gameProviderCode),
           externalRoundId: Arg.Is(command.Record.ExternalRoundId),
           gameId: Arg.Is(gameId ?? -1),
           externalGameId: Arg.Is(command.Record.ExternalGameId),
           playerCode: Arg.Is(command.Record.PlayerCode),
           walletId: Arg.Is(player!.DefaultWalletId),
           currencyCode: Arg.Is(player!.DefaultWalletCurrencyCode),
           brandId: Arg.Is(player!.BrandId),
           gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId!.Value),
           status: Arg.Is(RoundStatus.Open),
           clientStatus: Arg.Is(RoundClientStatus.Open),
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

        await fakeGameRoundsWorkClient
            .Received(1)
            .RoundMissing(
            gameProviderId: Arg.Is(gameProviderId),
            gameProviderCode: Arg.Is(gameProviderCode),
            externalRoundId: Arg.Is(command.Record.ExternalRoundId),
            gameId: Arg.Is(gameId ?? -1),
            externalGameId: Arg.Is(command.Record.ExternalGameId),
            playerCode: Arg.Is(command.Record.PlayerCode),
            data: Arg.Is(JsonSerializer.Serialize(command.Record)),
            cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Fact]
    public async void UnSettle()
    {
        var fakeGameRoundsWorkClient = Substitute.For<IGameRoundsWorkClient>();
        var fakeUtensil = Substitute.For<IUtensil>();

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

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "WGFM";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakeUtensil
            .GetPlayerAsync(
            playerCode: Arg.Is(command.Record.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        int? gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameExternalId: Arg.Is(command.Record.ExternalGameId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        Guid? gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var service = new HandleRecordCommandService(
            fakeUtensil,
            fakeGameRoundsWorkClient);

        await service
            .ExecuteAsync(
            command: command)
            .ConfigureAwait(false);

        await fakeGameRoundsWorkClient
           .Received(1)
           .RoundBidding(
           gameProviderId: Arg.Is(gameProviderId),
           gameProviderCode: Arg.Is(gameProviderCode),
           externalRoundId: Arg.Is(command.Record.ExternalRoundId),
           gameId: Arg.Is(gameId ?? -1),
           externalGameId: Arg.Is(command.Record.ExternalGameId),
           playerCode: Arg.Is(command.Record.PlayerCode),
           walletId: Arg.Is(player!.DefaultWalletId),
           currencyCode: Arg.Is(player!.DefaultWalletCurrencyCode),
           brandId: Arg.Is(player!.BrandId),
           gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId!.Value),
           status: Arg.Is(RoundStatus.Open),
           clientStatus: Arg.Is(RoundClientStatus.Open),
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

        await fakeGameRoundsWorkClient
            .Received(0)
            .RoundMissing(
            gameProviderId: Arg.Is(gameProviderId),
            gameProviderCode: Arg.Is(gameProviderCode),
            externalRoundId: Arg.Is(command.Record.ExternalRoundId),
            gameId: Arg.Is(gameId ?? -1),
            externalGameId: Arg.Is(command.Record.ExternalGameId),
            playerCode: Arg.Is(command.Record.PlayerCode),
            data: Arg.Is(JsonSerializer.Serialize(command.Record)),
            cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Fact]
    public async void Void()
    {
        var fakeGameRoundsWorkClient = Substitute.For<IGameRoundsWorkClient>();
        var fakeUtensil = Substitute.For<IUtensil>();

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

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "WGFM";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakeUtensil
            .GetPlayerAsync(
            playerCode: Arg.Is(command.Record.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        int? gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameExternalId: Arg.Is(command.Record.ExternalGameId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        Guid? gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var service = new HandleRecordCommandService(
            fakeUtensil,
            fakeGameRoundsWorkClient);

        await service
            .ExecuteAsync(
            command: command)
            .ConfigureAwait(false);

        await fakeGameRoundsWorkClient
           .Received(1)
           .RoundBidding(
           gameProviderId: Arg.Is(gameProviderId),
           gameProviderCode: Arg.Is(gameProviderCode),
           externalRoundId: Arg.Is(command.Record.ExternalRoundId),
           gameId: Arg.Is(gameId ?? -1),
           externalGameId: Arg.Is(command.Record.ExternalGameId),
           playerCode: Arg.Is(command.Record.PlayerCode),
           walletId: Arg.Is(player!.DefaultWalletId),
           currencyCode: Arg.Is(player!.DefaultWalletCurrencyCode),
           brandId: Arg.Is(player!.BrandId),
           gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId!.Value),
           status: Arg.Is(RoundStatus.Closed),
           clientStatus: Arg.Is(RoundClientStatus.Void),
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

        await fakeGameRoundsWorkClient
            .Received(0)
            .RoundMissing(
            gameProviderId: Arg.Is(gameProviderId),
            gameProviderCode: Arg.Is(gameProviderCode),
            externalRoundId: Arg.Is(command.Record.ExternalRoundId),
            gameId: Arg.Is(gameId ?? -1),
            externalGameId: Arg.Is(command.Record.ExternalGameId),
            playerCode: Arg.Is(command.Record.PlayerCode),
            data: Arg.Is(JsonSerializer.Serialize(command.Record)),
            cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }
}
