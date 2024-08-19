using NSubstitute;
using Tgs.GameRecord.WGFmGaming.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGaming.Core.UnitTest.ApplicationServices;

public class PullRecordCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
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
			fakeUnSettlePullRecordService,
			fakeSettleService);

		await service.ExecuteAsync(command: command);

		await fakeUnSettlePullRecordService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Any<PullRecordUnsettledCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		await fakeSettleService
			 .Received(1)
			 .ExecuteAsync(
			 command: Arg.Any<PullRecordSettledCommand>(),
			 cancellationToken: Arg.Any<CancellationToken>());
	}
}
