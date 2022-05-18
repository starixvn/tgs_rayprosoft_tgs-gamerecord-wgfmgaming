using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using Tgs.GameRecord.WGFMGamingService.ViewModels;
using Tgs.GameRecord.WGFMGamingService.Core.ApplicationServices;

namespace Tgs.GameRecord.WGFMGamingService.Controllers
{
    [ApiController]
    [Route("VR")]
    public class VRGamingController : ControllerBase
    {
        [HttpPost]
        public async ValueTask<GameRecordResultViewModel> GameRecordAsync(
            [FromServices] IGetService<GameRecordQuery, GameRecordResult> service,
            GameRecordQueryViewModel model)
        {
            var result = await service.GetAsync(new GameRecordQuery()
            {
                StartTime = model.StartTime,
                EndTime = model.EndTime
            }).ConfigureAwait(false);

            return new GameRecordResultViewModel()
            {
                Success = result.Success,
                LastGameRecorTime = result.LastGameRecorTime,
                SuccessCount = result.SuccessCount,
                FailCount = result.FailCount,
                Count = result.Count,
            };
        }
    }
}
