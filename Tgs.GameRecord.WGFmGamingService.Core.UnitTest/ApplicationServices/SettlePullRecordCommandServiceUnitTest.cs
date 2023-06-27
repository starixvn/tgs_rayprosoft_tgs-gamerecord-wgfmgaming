using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGamingService.Core.UnitTest.ApplicationServices;

public class SettlePullRecordCommandServiceUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        var fakeLogger = NullLogger<SettlePullRecordCommandService>.Instance;
        var fakeGameInterfaceWGFmGamingClient = Substitute.For<IGameInterfaceWGFmGamingClient>();
        var fakeService = Substitute.For<ICommandService<HandleRecordCommand>>();

        var command = new SettlePullRecordCommand(
            new Uri("http://localhost/"),
            PartnerCode: "partnerCode",
            PartnerKey: "partnerKey",
            PartnerIV: "partnerIV",
            StartTime: DateTime.UtcNow.AddMinutes(-10),
            EndTime: DateTime.UtcNow);

        var service = new SettlePullRecordCommandService(
            fakeLogger,
            fakeGameInterfaceWGFmGamingClient,
            fakeService);

        await service
            .ExecuteAsync(
            command: command)
            .ConfigureAwait(false);

        _ = await fakeGameInterfaceWGFmGamingClient
            .Received(1)
            .GameHistoryAsync(
            apiUrl: Arg.Is(command.ApiUrl),
            partnerCode: Arg.Is(command.PartnerCode),
            partnerKey: Arg.Is(command.PartnerKey),
            partnerIV: Arg.Is(command.PartnerIV),
            isSettle: Arg.Is(true),
            startTime: Arg.Is(command.StartTime),
            endTime: Arg.Is(command.EndTime),
            cancellationToken: Arg.Any<CancellationToken>())
            .ToArrayAsync(
            cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }
}
