﻿using Hangfire.Dashboard;

namespace Hangfire.Auth
{
    public class AuthenticatedAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public Boolean Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            return httpContext.User.Identity!.IsAuthenticated;
        }
    }
}