using Hangfire.Dashboard;

namespace Ethereal.WebAPI.Filters;

public class NoDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}