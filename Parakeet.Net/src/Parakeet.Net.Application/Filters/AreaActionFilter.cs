using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Volo.Abp.Caching;

namespace Parakeet.Net.Filters
{
    /// <summary>
    /// 区域filter
    /// </summary>
    public class AreaActionFilter : ActionFilterAttribute
    {
        private readonly ILogger<AreaActionFilter> _logger;
        private readonly IDistributedCache<string> _distributedCache;
        public AreaActionFilter(ILogger<AreaActionFilter> logger, IDistributedCache<string> distributedCache)
        {
            _logger = logger;
            _distributedCache = distributedCache;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ////http://chengdu.ops.spdai.com/home/index
            //var host = context.HttpContext.Request.Host.Value;
            //var cachKey = $"areaString_{context.ActionDescriptor.Id}";
            //var areaString = _distributedCache.GetOrAdd(cachKey, () => host.Split('.').FirstOrDefault()?.ToLower());

            //if (areaString.Contains("localhost"))
            //{
            //    areaString = areaString.Replace("localhost","chengdu");
            //}
            //var actionLog = $"{DateTime.Now} 请求path: {context.HttpContext.Request.Path},开始缓存key:{cachKey},value:{areaString}";
            //_logger.LogDebug(actionLog);
        }
    }
}
