using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading.Tasks;

namespace Parakeet.Net.Middleware
{
    /// <summary>
    /// app.UseMiddleware
    /// </summary>
    public class BowenMiddleware //不实现接口
    {
        private readonly RequestDelegate _next;//执行下一步的请求委托
        private string _messsage;
        /// <summary>
        /// 一定有个构造函数，传递RequestDelegate  源码做了检查的
        /// </summary>
        /// <param name="next"></param>
        public BowenMiddleware(RequestDelegate next,string message)
        {
            _next = next;
            _messsage = message;
        }

        /// <summary>
        /// 参数：构造函数可以有参数,且参数个数无限制，但第一个参数有类型限制，参数一般通过use中间件时传入，
        ///         如果use中间件时没有传入，build()时会去构造中间件，这时就会从IOC容器中去获取
        /// 约束：方法名称必须是Invoke 或者InvokeAsync  并且返回类型必须是Task  并且第一个参数为HttpContext类型
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context,ILogger<BowenMiddleware> _logger)
        {
            _logger.LogDebug($"message:{_messsage}");
            //中文输出不乱码  需要配置context的头
            if (context.Request.Path.Value.Contains("Bowen"))
            {
                await context.Response.WriteAsync($"{nameof(BowenMiddleware)}输入的字符串包含Bowen此分支不会继续执行{nameof(RequestDelegate)}" +
                 $"所以这里是Bowen的终结点<br/>", Encoding.UTF8);
            }
            else
            {
                await context.Response.WriteAsync($"{nameof(BowenMiddleware)} continue1");
                await _next(context);//俄罗斯套娃，继续先执行后面的逻辑
                await context.Response.WriteAsync($"{nameof(BowenMiddleware)} continue2");
            }
        }
    }
}
