using Tgs.GameRecord.WGFmGaming.Core.JobApplicationServices;

namespace Tgs.GameRecord.WGFmGaming.Core;

public interface IHangfireUtensilRepository
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
