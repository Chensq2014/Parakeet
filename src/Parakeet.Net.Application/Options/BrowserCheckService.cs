using Common.Dtos;
using Common.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Parakeet.Net.Middleware;
using System;
using System.Linq;

namespace Parakeet.Net.Options
{
    /// <summary>
    /// 浏览器检查服务
    /// </summary>
    public class BrowserCheckService : IBrowserCheck
    {
        //private readonly BrowserFilterOptions _browserFilterOptions;
        private readonly ILogger _logger;

        public BrowserCheckService(
            ILogger<BrowserCheckService> logger
        //, IOptionsMonitor<BrowserFilterOptions> browserFilterOptions
        )
        {
            _logger = logger;
            //_browserFilterOptions = browserFilterOptions.CurrentValue;
        }

        public Tuple<bool, string> CheckBrowser(HttpContext httpContext, BrowserFilterOptions options)
        {
            _logger.LogDebug($"EnableChorme={options.EnableChorme}");
            _logger.LogDebug($"EnableEdge={options.EnableEdge}");
            _logger.LogDebug($"EnableFirefox={options.EnableFirefox}");
            _logger.LogDebug($"EnableIE={options.EnableIE}");


            if (httpContext.Request.Headers["User-Agent"].Contains("Edg/") && !options.EnableEdge)
            {
                _logger.LogDebug($"{nameof(BrowserFilterMiddleware)} Refuse Edge,Choose other one<br/>");
                return Tuple.Create(false, $"{nameof(BrowserFilterMiddleware)} Refuse Edge,Choose other one<br/>");
            }
            else
            {
                return Tuple.Create(true, $"{nameof(BrowserFilterMiddleware)} ok");
            }
        }
        //User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36
        //User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36 Edg/91.0.864.64
    }
}
