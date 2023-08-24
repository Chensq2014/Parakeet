using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Parakeet.Net.Filters
{
    /// <summary>
    /// Action的Filter  ActionFilterAttribute:框架已经封装了一个全套给我们
    /// </summary>
    public class CustomCrossOriginActionFilterAttribute : ActionFilterAttribute//Attribute, IActionFilter
    {
        private readonly ILogger<CustomCrossOriginActionFilterAttribute> _logger;
        public CustomCrossOriginActionFilterAttribute(ILogger<CustomCrossOriginActionFilterAttribute> logger)
        {
            _logger = logger;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //context.HttpContext.Request.ContentType = "设置你的ContentType";
            _logger.LogDebug($"{nameof(CustomCrossOriginActionFilterAttribute)} Executing!");
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //设置浏览器Headers Access-Control-Allow-Origin为*允许跨域
            context.HttpContext.Response.Headers.TryAdd("Access-Control-Allow-Origin", "*");
            _logger.LogDebug($"{nameof(CustomCrossOriginActionFilterAttribute)} Executed!");
        }
    }
}
