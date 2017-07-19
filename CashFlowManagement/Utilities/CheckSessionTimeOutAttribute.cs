using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashFlowManagement.Utilities
{
    public class CheckSessionTimeOutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            var context = filterContext.HttpContext;
            if (context.Session.IsNewSession || context.Session["USER"] == null)
            {
                if (context.Request.IsAjaxRequest())
                {
                    context.Response.StatusCode = 401;
                    context.Response.End();
                }
                else
                {
                    string url = "~/Account/Login";
                    context.Response.Redirect(url);
                }
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }
}