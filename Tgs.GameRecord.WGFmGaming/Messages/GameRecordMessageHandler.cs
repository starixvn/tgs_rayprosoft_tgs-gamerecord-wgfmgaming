using System.Text;
using System.Text.Json;
using Tgs.GameRecord.WGFmGaming.Common;
using Tgs.GameRecord.WGFmGaming.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.WGFmGaming.Messages;

internal class GameRecordMessageHandler(
    ILogger<GameRecordMessageHandler> logger,
    ICommandService<PullRecordCommand> service) : IMessageHandler
{
    public ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var json = Encoding.UTF8.GetString(data.Span);
        var message = JsonSerializer.Deserialize<WGFmGamingGameRecordMessage>(json)
            ?? throw new NullReferenceException("The request data resolves to a null value");

        logger.LogInformation("LogOn:{logOn} Information Request:{message}", DateTime.Now.ToString("o"), message);

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
