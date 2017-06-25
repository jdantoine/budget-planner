using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace JDBudgetPlanner.Helpers
{
    public class AuthorizeHouseholdRequired : AuthorizeAttribute
    {
        //check if user is authorize and is in household
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }

            return httpContext.User.Identity.IsInHousehould();
        }

        //if user is not in household, redirect to join household
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Household", action = "JoinHousehold" }));
            }
            
        }
    }
}