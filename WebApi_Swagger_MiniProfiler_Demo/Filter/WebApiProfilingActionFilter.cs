using Newtonsoft.Json;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace WebApi_Swagger_MiniProfiler_Demo.Filter
{
    public class WebApiProfilingActionFilter : ActionFilterAttribute
    {
        public const string MiniProfilerResultsHeaderName = "X-MiniProfiler-Ids";

        public override void OnActionExecuted(HttpActionExecutedContext filterContext)
        {
            var MiniProfilerJson = JsonConvert.SerializeObject(new[] { MiniProfiler.Current.Id });
            filterContext.Response.Content.Headers.Add(MiniProfilerResultsHeaderName, MiniProfilerJson);
        }
    }
}