using Tgs.GameRecord.WGFmGaming.Core.JobApplicationServices;

namespace Tgs.GameRecord.WGFmGaming.Core;

public interface IHangfireUtensil
{
    public Task JobCheckAsync();

    public Task JobExecuteAsync(JobExecuteCommand source);

    public Task JobInitAsync();
}
