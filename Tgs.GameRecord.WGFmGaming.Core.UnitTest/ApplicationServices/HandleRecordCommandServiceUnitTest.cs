using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.WGFmGamingService.Protos;
using Tgs.GameRecord.WGFmGaming.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.WGFmGaming.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var gameRecordInfo = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "WGFM",
			GameProviderConfiguration = "WGFMConfiguration",
			HangfirePrefix = "gamerecord-wgfm-hangfire:",
			HangfireLocking = "gamerecord-wgfm-locking:",
			IntervalMinutes = 3,
			IntervalMinutesMax = 1440,
			IntervalMinutesMinimum = 1,
			LockMinute = 2
		});
		var fakeMessageSender = Substitute.For<IMessageSender>();

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
			faceLogger,
			gameRecordInfo,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
			data: Arg.Is(new Handle.Common.Protos.GameRecordHandleRequest
			{
				GameProviderCode = gameRecordInfo.Value.GameProviderCode,
				PlayerCode = command.Record.PlayerCode,
				ExternalRoundId = command.Record.ExternalRoundId,
				ExternalBetId = command.Record.ExternalRoundId,
				ExternalGameId = command.Record.ExternalGameId,
				ExternalCreatedOn = DateTimeOffset.Parse(command.Record.ExternalCreatedOn).ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.Parse(command.Record.ExternalClosedOn).ToTimestamp(),
				ExternalPayoffOn = DateTimeOffset.Parse(command.Record.ExternalClosedOn).ToTimestamp(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
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
		var gameRecordInfo = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "WGFM",
			GameProviderConfiguration = "WGFMConfiguration",
			HangfirePrefix = "gamerecord-wgfm-hangfire:",
			HangfireLocking = "gamerecord-wgfm-locking:",
			IntervalMinutes = 3,
			IntervalMinutesMax = 1440,
			IntervalMinutesMinimum = 1,
			LockMinute = 2
		});
		var fakeMessageSender = Substitute.For<IMessageSender>();

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
			faceLogger,
			gameRecordInfo,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
			data: Arg.Is(new Handle.Common.Protos.GameRecordHandleRequest
			{
				GameProviderCode = gameRecordInfo.Value.GameProviderCode,
				PlayerCode = command.Record.PlayerCode,
				ExternalRoundId = command.Record.ExternalRoundId,
				ExternalBetId = command.Record.ExternalRoundId,
				ExternalGameId = command.Record.ExternalGameId,
				ExternalCreatedOn = DateTimeOffset.Parse(command.Record.ExternalCreatedOn).ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				ExternalPayoffOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
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
		var gameRecordInfo = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "WGFM",
			GameProviderConfiguration = "WGFMConfiguration",
			HangfirePrefix = "gamerecord-wgfm-hangfire:",
			HangfireLocking = "gamerecord-wgfm-locking:",
			IntervalMinutes = 3,
			IntervalMinutesMax = 1440,
			IntervalMinutesMinimum = 1,
			LockMinute = 2
		});
		var fakeMessageSender = Substitute.For<IMessageSender>();

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
			faceLogger,
			gameRecordInfo,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
			data: Arg.Is(new Handle.Common.Protos.GameRecordHandleRequest
			{
				GameProviderCode = gameRecordInfo.Value.GameProviderCode,
				PlayerCode = command.Record.PlayerCode,
				ExternalRoundId = command.Record.ExternalRoundId,
				ExternalBetId = command.Record.ExternalRoundId,
				ExternalGameId = command.Record.ExternalGameId,
				ExternalCreatedOn = DateTimeOffset.Parse(command.Record.ExternalCreatedOn).ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.Parse(command.Record.ExternalClosedOn).ToTimestamp(),
				ExternalPayoffOn = DateTimeOffset.Parse(command.Record.ExternalClosedOn).ToTimestamp(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
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
