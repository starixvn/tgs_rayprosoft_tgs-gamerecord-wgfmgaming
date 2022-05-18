using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.WGFmGamingService.Core.ApplicationServices;
using Tgs.GameRecord.WGFmGamingService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WGFmGamingService.Controllers
{
    [ApiController]
    [Route("WGFM")]
    public class WGFmGamingController : ControllerBase
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
