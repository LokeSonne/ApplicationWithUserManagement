using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoniReports.Tools
{
    /// <summary>
    /// Expose cause of Internal Server errors 
    /// </summary>
    public class HandleErrorWithAjaxFilter : HandleErrorAttribute
    {
        public bool ShowStackTraceIfNotDebug { get; set; }

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var content = ShowStackTraceIfNotDebug ||
                                filterContext.HttpContext.IsDebuggingEnabled ?
                                    filterContext.Exception.StackTrace :
                                    string.Empty;

                filterContext.Result = new ContentResult
                {
                    ContentType = "text/plain",//Thanks Colin
                    Content = content
                };

                filterContext.HttpContext.Response.Status =
                    "500 " + filterContext.Exception.Message
                    .Replace("\r", " ")
                    .Replace("\n", " ");
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
            else
            {
                base.OnException(filterContext);
            }
        }
    }
}