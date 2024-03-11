using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Parakeet.Net.Filters
{
    /// <summary>
    /// 放在全局
    /// </summary>
    public class CustomGlobalActionFilterAttribute : ActionFilterAttribute// Attribute, IActionFilter
    {
        private readonly ILogger<CustomGlobalActionFilterAttribute> _logger = null;
        public CustomGlobalActionFilterAttribute(ILogger<CustomGlobalActionFilterAttribute> logger)
        {
            this._logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            this._logger.LogDebug($"{nameof(CustomGlobalActionFilterAttribute)} Executing!");
            //_logger.LogDebug($"{DateTime.Now} 开始调用 {context.RouteData.Values["action"]} api；参数为：{Newtonsoft.Json.JsonConvert.SerializeObject(context.ActionArguments)}");
            //context.HttpContext.Response.WriteAsync("CustomGlobalActionFilterAttribute Executing!");
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //var result = context.Result;
            //var objectResult = result as ObjectResult;
            //var resultLog = $"{DateTime.Now} 调用 {context.RouteData.Values["action"]} api 完成；执行结果：{Newtonsoft.Json.JsonConvert.SerializeObject(objectResult?.Value)}";
            //_logger.LogDebug(resultLog);
            this._logger.LogDebug($"{nameof(CustomGlobalActionFilterAttribute)} Executed!");
            //context.HttpContext.Response.WriteAsync("CustomGlobalActionFilterAttribute Executed!");
        }
    }
}
