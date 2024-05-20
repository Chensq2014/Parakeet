using Common.Dtos;
using Common.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

namespace Parakeet.Net.Filters
{
    /// <summary>
    /// 异常处理的filter 
    /// </summary>
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly ILogger<CustomExceptionFilterAttribute> _logger;

        /// <summary>
        /// ioc
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        /// <param name="modelMetadataProvider"></param>
        /// <param name="logger"></param>
        public CustomExceptionFilterAttribute(
            IWebHostEnvironment hostingEnvironment,
            IModelMetadataProvider modelMetadataProvider,
            ILogger<CustomExceptionFilterAttribute> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _modelMetadataProvider = modelMetadataProvider;
            _logger = logger;
        }

        /// <summary>
        /// 没有处理的异常，就会进来
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnException(ExceptionContext filterContext)
        {
            _logger.LogError($"{{0}}", $"{CacheKeys.LogCount++}、{filterContext.Exception.Message}");//记录下错误消息
            if (!filterContext.ExceptionHandled)//异常没有被处理过
            {
                //string controllerName = (string) filterContext.RouteData.Values["controller"];
                //string actionName = (string) filterContext.RouteData.Values["action"];
                //string msgTemplate = $"在执行 controller[{controllerName}] 的 action[{actionName}] 时产生异常";
                //_logger.LogError(msgTemplate, filterContext.Exception);
                //检查请求头
                if (this.IsAjaxRequest(filterContext.HttpContext.Request))
                {
                    filterContext.Result = new JsonResult(new AjaxReturnMessage
                    {
                        Status = false,
                        Msg = "系统出现异常，请联系管理员",
                        ErrorMsg = filterContext.Exception.Message
                    });//这个就是返回的结果
                }
                else
                {
                    var result = new ViewResult
                    {
                        ViewName = "~/Views/Shared/Error.cshtml",
                        ViewData = new ViewDataDictionary(_modelMetadataProvider, filterContext.ModelState)
                        {
                            {"Exception", filterContext.Exception}
                        }
                    };
                    filterContext.Result = result;
                }
                //标记当前抛出的异常已经被处理过了
                filterContext.ExceptionHandled = true;
            }
        }

        /// <summary>
        /// 检查是否ajax请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool IsAjaxRequest(HttpRequest request)
        {
            string header = request.Headers["X-Requested-With"];
            return "XMLHttpRequest".Equals(header);
        }

    }
}
