using Hangfire.Dashboard;

namespace Tgs.GameRecord.WGFmGaming;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context) => true;
}
