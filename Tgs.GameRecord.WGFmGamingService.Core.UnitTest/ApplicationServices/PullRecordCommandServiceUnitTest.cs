using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGamingService.Core.UnitTest.ApplicationServices;

public class PullRecordCommandServiceUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        var fakeLogger = NullLogger<PullRecordCommandService>.Instance;
        var fakeUnSettlePullRecordService = Substitute.For<ICommandService<UnSettlePullRecordCommand>>();
        var fakeSettleService = Substitute.For<ICommandService<SettlePullRecordCommand>>();

        var command = new PullRecordCommand(
            new Uri("http://localhost/"),
            PartnerCode: "partnerCode",
            PartnerKey: "partnerKey",
            PartnerIV: "partnerIV",
            StartTime: DateTime.UtcNow.AddMinutes(-10),
            EndTime: DateTime.UtcNow);

        var service = new PullRecordCommandService(
            fakeLogger,
            fakeUnSettlePullRecordService,
            fakeSettleService);

        await service
            .ExecuteAsync(
            command: command)
            .ConfigureAwait(false);

        await fakeUnSettlePullRecordService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Any<UnSettlePullRecordCommand>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);

        await fakeSettleService
             .Received(1)
             .ExecuteAsync(
             command: Arg.Any<SettlePullRecordCommand>(),
             cancellationToken: Arg.Any<CancellationToken>())
             .ConfigureAwait(false);
    }
}
