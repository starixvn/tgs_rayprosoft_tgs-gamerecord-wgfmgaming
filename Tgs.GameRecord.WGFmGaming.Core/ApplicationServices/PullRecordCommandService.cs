using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGaming.Core.ApplicationServices;

internal class PullRecordCommandService(
	ICommandService<PullRecordUnsettledCommand> unSettleService,
	ICommandService<PullRecordSettledCommand> settleService) : ICommandService<PullRecordCommand>
{
	public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
	{
		await unSettleService.ExecuteAsync(
			command: new PullRecordUnsettledCommand(
				ApiUrl: command.ApiUrl,
				PartnerCode: command.PartnerCode,
				PartnerKey: command.PartnerKey,
				PartnerIV: command.PartnerIV,
				StartTime: command.StartTime,
				EndTime: command.EndTime),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await settleService.ExecuteAsync(
			command: new PullRecordSettledCommand(
				ApiUrl: command.ApiUrl,
				PartnerCode: command.PartnerCode,
				PartnerKey: command.PartnerKey,
				PartnerIV: command.PartnerIV,
				StartTime: command.StartTime,
				EndTime: command.EndTime),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
