using Hangfire.Dashboard;

namespace RaffleKing.Infrastructure;

public class AllowAllAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true; // development purposes only
    }
}