using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;
using Tgs.GameRecord.WGFmGamingService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGamingService.Controllers;

[ApiController]
[Route("WGFM")]
public class WGFmGamingController : ControllerBase
{
    [HttpPost]
    public ValueTask GameRecordAsync(
        [FromServices] ICommandService<SettlePullRecordCommand> service,
        GameRecordQueryViewModel source)
        => service
            .ExecuteAsync(
            new SettlePullRecordCommand(
                    source.ApiUrl,
                    source.PartnerCode,
                    source.PartnerKey,
                    source.PartnerIV,
                    source.StartTime,
                    source.EndTime));
}
