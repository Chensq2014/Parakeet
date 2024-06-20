using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Parakeet.Net.Filters
{
    /// <summary>
    /// 放在controller上 filter
    /// </summary>
    public class CustomControllerActionFilterAttribute : ActionFilterAttribute  //Attribute, IActionFilter
    {
        private readonly ILogger<CustomControllerActionFilterAttribute> _logger;
        public CustomControllerActionFilterAttribute(ILogger<CustomControllerActionFilterAttribute> logger)
        {
            this._logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            this._logger.LogDebug($"{nameof(CustomControllerActionFilterAttribute)} Executing!");
            //_logger.LogDebug($"{DateTime.Now} 开始调用 {context.RouteData.Values["action"]} api；参数为：{Newtonsoft.Json.JsonConvert.SerializeObject(context.ActionArguments)}");
            //context.HttpContext.Response.WriteAsync("CustomControllerActionFilterAttribute Executing!");
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //var result = context.Result;
            //var objectResult = result as ObjectResult;
            //var resultLog = $"{DateTime.Now} 调用 {context.RouteData.Values["action"]} api 完成；执行结果：{Newtonsoft.Json.JsonConvert.SerializeObject(objectResult?.Value)}";
            //_logger.LogDebug(resultLog);
            this._logger.LogDebug($"{nameof(CustomControllerActionFilterAttribute)} Executed!");
            //context.HttpContext.Response.WriteAsync("CustomControllerActionFilterAttribute Executed!");
        }
    }
}
