using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ocelot.Service
{
    public class OcelotAuthorizationAttribute : AuthorizeAttribute
    {

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetNoServerCaching();
            filterContext.HttpContext.Response.Cache.SetNoStore();

            var userId = Constant.GetUserID();
            string roles = base.Roles;
           
            if (base.Roles == "LOGIN")
            {
                filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            }
            else if(base.Roles == "LOGOUT")
            {
                filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            }
            else
            {
                if (userId == "")
                {
                    base.HandleUnauthorizedRequest(filterContext);
                }
                bool state = ClaimsManager.IsUserInRole(roles);

                if (!state)
                {
                    filterContext.Result = new Http403Result();
                }
                else
                {
                    filterContext.HttpContext.Response.StatusCode = 200;
                    
                }
            }
            
        }
    }
    internal class Http403Result : ActionResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            var routes = new RouteCollection();
            context.HttpContext.Response.RedirectPermanent("/Error/AccesDenied");
        }
    }
}