using Common.Dtos;
using Common.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Parakeet.Net.Middleware
{
    /// <summary>
    /// 浏览器检查中间件
    /// </summary>
    public class BrowserFilterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IBrowserCheck _iBrowserCheck;
        private readonly BrowserFilterOptions _browserFilterOptions;
        private readonly BrowserFilterOptions _browserFilterMonitorOptions;

        ///// <summary>
        ///// IOptions<BrowserFilterOptions> options 有2个来源
        ///// 既可以Use的时候去直接传递
        ///// 也可以Add是Configure,这里再获取
        ///// </summary>
        ///// <param name="next"></param>
        ///// <param name="logger"></param>
        ///// <param name="browserCheck"></param>
        ///// <param name="options"></param>

        //public BrowserFilterMiddleware(RequestDelegate next,
        //    ILogger<BrowserFilterMiddleware> logger, 
        //    IBrowserCheck browserCheck, 
        //    //IOptionsMonitor<BrowserFilterOptions> optionMonitor, 
        //    IOptions<BrowserFilterOptions> options)
        //{
        //    this._next = next;
        //    this._logger = logger;
        //    this._iBrowserCheck = browserCheck;
        //    this._browserFilterOptions = options.Value;
        //    //this._browserFilterOptions = optionMonitor.CurrentValue;
        //}

        /// <summary>
        /// 玩法3一起生效---先Use为准，再叠加Add的委托
        /// 这不是框架的推荐写法，仅用于技术研究和融合应用，需仔细验证
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        /// <param name="browserCheck"></param>
        /// <param name="options"></param>
        public BrowserFilterMiddleware(RequestDelegate next, 
            ILogger<BrowserFilterMiddleware> logger, 
            IBrowserCheck browserCheck, 
            IConfigureOptions<BrowserFilterOptions> configureNamedOptions, //从add中已经配置好的BrowserFilterOptions单例类中获取
            IOptionsMonitor<BrowserFilterOptions> optionMonitor,
            IOptions<BrowserFilterOptions> options)
        {
            this._next = next;
            this._logger = logger;
            this._iBrowserCheck = browserCheck;
            this._browserFilterOptions = options.Value;//options.Value会以use时传入的BrowserFilterOptions为准,如果use没传才会去IOC中获取IConfigureOptions<BrowserFilterOptions>
            configureNamedOptions.Configure(options.Value);//玩法3一起生效---先Use为准，再叠加Add的最后一个BrowserFilterOptions委托
            this._browserFilterMonitorOptions = optionMonitor.CurrentValue;//optionMonitor.CurrentValue 会执行所有add时的委托内容，以add为准
        }

        /// <summary>
        /// 方法名字Invoke或者InvokeAsync
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var result = this._iBrowserCheck.CheckBrowser(context, _browserFilterOptions);
            if (!result.Item1)
            {
                _logger.LogDebug($"{nameof(BrowserFilterMiddleware)} {result.Item2}");
                await context.Response.WriteAsync($"{nameof(BrowserFilterMiddleware)} {result.Item2}");
            }
            else
            {
                _logger.LogDebug($"{nameof(BrowserFilterMiddleware)} ok");
                await _next(context);
            }
        }
    }
}
