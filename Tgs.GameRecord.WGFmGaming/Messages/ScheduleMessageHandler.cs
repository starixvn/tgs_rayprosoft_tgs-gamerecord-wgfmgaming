using System.Text;
using System.Text.Json;
using Tgs.GameRecord.WGFmGaming.Common;
using Tgs.GameRecord.WGFmGaming.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.WGFmGaming.Messages;

internal class ScheduleMessageHandler(
    ILogger<ScheduleMessageHandler> logger,
    ICommandService<PullRecordCommand> service) : IMessageHandler<ReadOnlyMemory<byte>>
{
	public ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var json = Encoding.UTF8.GetString(data.Span);
		var message = JsonSerializer.Deserialize<WGFmGamingGameRecordMessage>(json)
			?? throw new NullReferenceException("The request data resolves to a null value");

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Information Request:{message}",
			DateTime.Now.ToString("o"),
			subject,
			message);

		var command = new PullRecordCommand(
			ApiUrl: message.ApiUrl,
			PartnerCode: message.PartnerCode,
			PartnerKey: message.PartnerKey,
			PartnerIV: message.PartnerIV,
			StartTime: message.StartTime,
			EndTime: message.EndTime);

		return service.ExecuteAsync(
			command: command,
			cancellationToken: cancellationToken);
	}
}
