using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.WGFmGamingService.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.WGFmGaming.Core.ApplicationServices;

namespace Tgs.GameRecord.WGFmGaming.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakeGameRecordInfoOptions = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "WGFM",
			GameProviderConfiguration = "WGFMConfiguration",
			GameExternalId = "1",
			HangfirePrefix = "gamerecord-wgfm-hangfire:",
			HangfireLocking = "gamerecord-wgfm-locking:",
			IntervalMinutes = 3,
			IntervalMinutesMax = 1440,
			IntervalMinutesMinimum = 1,
			LockMinute = 2
		});
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

		var command = new HandleRecordCommand(
			Record: new GameHistoryInfo
			{
				ExternalRoundId = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
				ExternalGameId = "sports_book",
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

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakeGameRecordInfoOptions,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = fakeGameRecordInfoOptions.Value.GameProviderCode,
				PlayerCode = command.Record.PlayerCode,
				ExternalRoundId = command.Record.ExternalRoundId,
				ExternalBetId = command.Record.ExternalRoundId,
				ExternalGameId = fakeGameRecordInfoOptions.Value.GameExternalId,
				ExternalCreatedOn = DateTimeOffset.Parse(command.Record.ExternalCreatedOn),
				ExternalClosedOn = DateTimeOffset.Parse(command.Record.ExternalClosedOn),
				ExternalPayoffOn = DateTimeOffset.Parse(command.Record.ExternalClosedOn),
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Win,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 10m,
				ValidBet = 5m,
				Settle = 7.5m,
				Commission = 0.0M,
				Turnover = 5m,
				WinLoss = 2.5m,
				Jackpot = 0.0M,
				AdditionalData = command.Record.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task UnSettle()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakeGameRecordInfoOptions = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "WGFM",
			GameProviderConfiguration = "WGFMConfiguration",
			GameExternalId = "1",
			HangfirePrefix = "gamerecord-wgfm-hangfire:",
			HangfireLocking = "gamerecord-wgfm-locking:",
			IntervalMinutes = 3,
			IntervalMinutesMax = 1440,
			IntervalMinutesMinimum = 1,
			LockMinute = 2
		});
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

		var command = new HandleRecordCommand(
			Record: new GameHistoryInfo
			{
				ExternalRoundId = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
				ExternalGameId = "sports_book",
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

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakeGameRecordInfoOptions,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = fakeGameRecordInfoOptions.Value.GameProviderCode,
				PlayerCode = command.Record.PlayerCode,
				ExternalRoundId = command.Record.ExternalRoundId,
				ExternalBetId = command.Record.ExternalRoundId,
				ExternalGameId = fakeGameRecordInfoOptions.Value.GameExternalId,
				ExternalCreatedOn = DateTimeOffset.Parse(command.Record.ExternalCreatedOn),
				ExternalClosedOn = DateTimeOffset.UnixEpoch,
				ExternalPayoffOn = DateTimeOffset.UnixEpoch,
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Running,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 10m,
				ValidBet = 5m,
				Settle = 0.0m,
				Commission = 0.0M,
				Turnover = 0.0m,
				WinLoss = 0.0m,
				Jackpot = 0.0M,
				AdditionalData = command.Record.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Void()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakeGameRecordInfoOptions = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "WGFM",
			GameProviderConfiguration = "WGFMConfiguration",
			GameExternalId = "1",
			HangfirePrefix = "gamerecord-wgfm-hangfire:",
			HangfireLocking = "gamerecord-wgfm-locking:",
			IntervalMinutes = 3,
			IntervalMinutesMax = 1440,
			IntervalMinutesMinimum = 1,
			LockMinute = 2
		});
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

		var command = new HandleRecordCommand(
			Record: new GameHistoryInfo
			{
				ExternalRoundId = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
				ExternalGameId = "sports_book",
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

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakeGameRecordInfoOptions,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = fakeGameRecordInfoOptions.Value.GameProviderCode,
				PlayerCode = command.Record.PlayerCode,
				ExternalRoundId = command.Record.ExternalRoundId,
				ExternalBetId = command.Record.ExternalRoundId,
				ExternalGameId = fakeGameRecordInfoOptions.Value.GameExternalId,
				ExternalCreatedOn = DateTimeOffset.Parse(command.Record.ExternalCreatedOn),
				ExternalClosedOn = DateTimeOffset.Parse(command.Record.ExternalClosedOn),
				ExternalPayoffOn = DateTimeOffset.Parse(command.Record.ExternalClosedOn),
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Void,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 0.0M,
				ValidBet = 0.0M,
				Settle = 0.0m,
				Commission = 0.0M,
				Turnover = 0.0m,
				WinLoss = 0.0m,
				Jackpot = 0.0M,
				AdditionalData = command.Record.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
