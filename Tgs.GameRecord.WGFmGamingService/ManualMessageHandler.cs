using System.Text;
using System.Text.Json;
using Tgs.GameRecord.WGFmGamingService.Core.WorkerServices;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGamingService;

public class ManualMessageHandler : IMessageProcessor
{
    private readonly ICommandService<SettleWorkerCommand> _service;

    public ManualMessageHandler(ICommandService<SettleWorkerCommand> service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var json = Encoding.UTF8.GetString(data.Span);
        var request = JsonSerializer.Deserialize<ManualMessageRequest>(json)!;

        await _service.ExecuteAsync(new SettleWorkerCommand(
           ApiUrl: request.ApiUrl,
           PartnerCode: request.PartnerCode,
           PartnerKey: request.PartnerKey,
           partnerIV: request.PartnerIV,
           StartTime: request.StartTime,
           EndTime: request.EndTime), cancellationToken).ConfigureAwait(false);

        var response = new ManualMessageResponse()
        {
            Success = true
        };

        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
    }
}
