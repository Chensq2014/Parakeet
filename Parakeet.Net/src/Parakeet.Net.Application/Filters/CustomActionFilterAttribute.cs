using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace Parakeet.Net.Filters
{
    /// <summary>
    /// Action的Filter  ActionFilterAttribute:框架已经封装了一个全套给我们
    /// </summary>
    public class CustomActionFilterAttribute : ActionFilterAttribute//Attribute, IActionFilter
    {
        private readonly ILogger<CustomActionFilterAttribute> _logger = null;
        public CustomActionFilterAttribute(ILogger<CustomActionFilterAttribute> logger)
        {
            _logger = logger;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //判断filter的实现类 特殊处理
            //var test = context.ActionDescriptor.FilterDescriptors
            //    .Select(m => m.Filter)
            //    .OfType<TypeFilterAttribute>()
            //    .Any(n => n.ImplementationType == typeof(CustomActionFilterAttribute));

            _logger.LogDebug($"{nameof(CustomActionFilterAttribute)}  Executing!");
            _logger.LogDebug($"{DateTime.Now} 开始调用 {context.RouteData.Values["action"]} api；参数为：{Newtonsoft.Json.JsonConvert.SerializeObject(context.ActionArguments)}");
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var result = context.Result;
            var objectResult = result as ObjectResult;
            var resultLog = $"{DateTime.Now} 调用 {context.RouteData.Values["action"]} api 完成；执行结果：{Newtonsoft.Json.JsonConvert.SerializeObject(objectResult?.Value)}";
            _logger.LogDebug(resultLog);
            _logger.LogDebug($"{nameof(CustomActionFilterAttribute)}  Executed!");
        }
    }
}
