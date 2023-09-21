using System.Text;
using System.Text.Json;
using Tgs.GameRecord.WGFmGaming.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.WGFmGaming.Messages;

public class ManualMessageHandler : IMessageProcessor
{
    private readonly ICommandService<PullRecordCommand> _service;

    public ManualMessageHandler(
        ICommandService<PullRecordCommand> service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var json = Encoding.UTF8.GetString(data.Span);
        var request = JsonSerializer.Deserialize<ManualMessageRequest>(json)!;

        var command = new PullRecordCommand(
            ApiUrl: request.ApiUrl,
            PartnerCode: request.PartnerCode,
            PartnerKey: request.PartnerKey,
            PartnerIV: request.PartnerIV,
            StartTime: request.StartTime,
            EndTime: request.EndTime);
        await _service
            .ExecuteAsync(
                command: command,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var response = new ManualMessageResponse()
        {
            Success = true
        };

        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
    }
}
