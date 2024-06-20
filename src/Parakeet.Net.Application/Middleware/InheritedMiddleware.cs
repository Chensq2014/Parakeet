using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Parakeet.Net.Middleware
{
    /// <summary>
    /// 1  实现IMiddleware，就不能有参数--而且还得IOC注册
    /// 2  IMiddleware的实现类型的初始化是请求来了之后才发生
    /// 3  如果响应请求时，才实例化，用完立即释放---这种就应该实现IMiddleware
    /// </summary>
    public class InheritedMiddleware: IMiddleware, IDisposable
    {
        private readonly ILogger _logger;

        public InheritedMiddleware(ILogger<InheritedMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _logger.LogDebug($"{nameof(InheritedMiddleware)},Hello World1!<br/>");

            //await context.Response.WriteAsync($"{nameof(InheritedMiddleware)},Hello World1!<br/>");
            await next(context);
            //await context.Response.WriteAsync($"{nameof(InheritedMiddleware)},Hello World2!<br/>");
        }

        public void Dispose()
        {
            _logger.LogDebug($"释放需要释放的资源");
        }
    }
}
