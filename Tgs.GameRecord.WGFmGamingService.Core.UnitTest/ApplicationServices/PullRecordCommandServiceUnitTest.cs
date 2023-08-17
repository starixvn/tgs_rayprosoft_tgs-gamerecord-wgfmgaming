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
        var fakeUnSettlePullRecordService = Substitute.For<ICommandService<PullRecordUnsettledCommand>>();
        var fakeSettleService = Substitute.For<ICommandService<PullRecordSettledCommand>>();

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
            command: Arg.Any<PullRecordUnsettledCommand>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);

        await fakeSettleService
             .Received(1)
             .ExecuteAsync(
             command: Arg.Any<PullRecordSettledCommand>(),
             cancellationToken: Arg.Any<CancellationToken>())
             .ConfigureAwait(false);
    }
}
