using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.WGFmGamingService.Core.WorkerServices;
using Tgs.GameRecord.WGFmGamingService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGamingService.Controllers;

[ApiController]
[Route("WGFM")]
public class WGFmGamingController : ControllerBase
{
    [HttpPost]
    public ValueTask GameRecordAsync(
        [FromServices] ICommandService<SettleWorkerCommand> service,
        GameRecordQueryViewModel source)
        => service
            .ExecuteAsync(
            new SettleWorkerCommand(
                    source.ApiUrl,
                    source.PartnerCode,
                    source.PartnerKey,
                    source.PartnerIV,
                    source.StartTime,
                    source.EndTime));
}
