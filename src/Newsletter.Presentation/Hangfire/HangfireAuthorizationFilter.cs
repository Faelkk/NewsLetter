using Hangfire.Dashboard;

namespace Newsletter.Presentation.Hangfire;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // var httpContext = context.GetHttpContext();
        //
        // return httpContext.User.Identity?.IsAuthenticated == true &&
        //        httpContext.User.IsInRole("Admin");
        
        return true; 
    }
}
