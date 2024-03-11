using Common.Storage;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace Parakeet.Net.Middleware
{
    /// <summary>
    /// app.UseMiddleware 自定义输出日志信息中间件
    /// </summary>
    public class CustomLogMiddleware
    {
        private readonly RequestDelegate _next;//执行下一步的请求委托

        public CustomLogMiddleware(RequestDelegate next)
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、app.UseMiddleware 自定义输出日志信息中间件{nameof(CustomLogMiddleware)}被构造....线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、【MVC流程日志：请求进入中间件{nameof(CustomLogMiddleware)} 下一步将进入---EndpointRoutingMiddleware开始路由匹配 ....】....线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            await _next(context);//俄罗斯套娃，继续先执行后面的逻辑
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、 【MVC流程日志：请求执行完毕，已回流 EndpointRoutingMiddleware完毕,中间件{nameof(CustomLogMiddleware)}处理请求完毕 ....】 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        }
    }
}
