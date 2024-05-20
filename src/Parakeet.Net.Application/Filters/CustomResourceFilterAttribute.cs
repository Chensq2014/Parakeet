using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using Common.Extensions;
using Volo.Abp.Caching;

namespace Parakeet.Net.Filters
{
    /// <summary>
    /// 自定义的资源Filter
    /// </summary>
    public class CustomResourceFilterAttribute : Attribute, IResourceFilter,IFilterFactory
    {
        //private static readonly Dictionary<string, IActionResult> _cache = new Dictionary<string, IActionResult>();
        private string _cacheKey;
        private readonly IDistributedCache<IActionResult> _cacheManager;
        private readonly Type _filterType = null;
        private readonly ILogger<CustomResourceFilterAttribute> _logger;
        public bool IsReusable => true;

        public CustomResourceFilterAttribute(IDistributedCache<IActionResult> cacheManager, ILogger<CustomResourceFilterAttribute> logger)
        {
            _cacheManager = cacheManager;
            _logger = logger;
        }
        
        /// <summary>
        /// 创建filter实例
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return (IFilterMetadata)serviceProvider.GetService(this._filterType);
        }

        /// <summary>
        /// 控制器实例化之前
        /// </summary>
        /// <param name="context"></param>
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            _logger.LogDebug($"{nameof(CustomResourceFilterAttribute)} OnResourceExecuting.....");
            _cacheKey = context.HttpContext.Request.Path.ToString();
            var cacheValue = _cacheManager.Get(_cacheKey);
            if (cacheValue != null)
            {
                Log.Logger.Information($"{nameof(CustomResourceFilterAttribute)} 使用缓存.....");
                //直接指定Result
                context.Result = cacheValue;//new ContentResult { Content = cacheValue };
            }

            //_cacheKey = context.HttpContext.Request.Path.ToString();
            //if (_cache.ContainsKey(_cacheKey))
            //{
            //    if (_cache[_cacheKey] is ViewResult cacheValue)
            //    {
            //        //直接指定Result
            //        context.Result = cacheValue;
            //    }
            //}
        }

        /// <summary>
        /// 把请求都处理完以后
        /// </summary>
        /// <param name="context"></param>
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            if (_cacheKey.HasValue() && _cacheManager.Get(_cacheKey) is null)
            {
                _logger.LogDebug($"{nameof(CustomResourceFilterAttribute)} 设置缓存.....");
                _cacheManager.Set(_cacheKey,context.Result);
            }

            //if (!string.IsNullOrWhiteSpace(_cacheKey) && !_cache.ContainsKey(_cacheKey))
            //{
            //    var result = context.Result;
            //    if (result != null)
            //    {
            //        _cache.Add(_cacheKey, result);
            //    }
            //}

            _logger.LogDebug($"{nameof(CustomResourceFilterAttribute)} OnResourceExecuted.....");
        }

    }
}
