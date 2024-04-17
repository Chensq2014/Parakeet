using Microsoft.AspNetCore.Builder;
using Common.Dtos;
using Parakeet.Net.Middleware;
using System;

namespace Common.Extensions
{
    /// <summary>
    /// use中间件扩展
    /// </summary>
    public static class UseMiddlewareExtention
    {
        /// <summary>
        /// use IMiddleware示例中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IApplicationBuilder UseInheritedMiddleware(this IApplicationBuilder app)
        {
            if (app is null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            //这里use不能传递参数 InheritedMiddleware构造函数只能有IOC容器注册的参数
            return app.UseMiddleware<InheritedMiddleware>();
        }

        /// <summary>
        /// use 未定义接口中间件示例
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IApplicationBuilder UseBowenMiddleware(this IApplicationBuilder app)
        {
            if (app is null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            //return app.UseMiddleware<BowenMiddleware>();
            return app.UseMiddleware<BowenMiddleware>("中间件参数传递");
        }

        /// <summary>
        /// use安全策略中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IApplicationBuilder UseSecurePolicy(this IApplicationBuilder app)
        {
            if (app is null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<SecurePolicyMiddleware>();
        }

        /// <summary>
        /// use自定义日志中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IApplicationBuilder UseCustomLog(this IApplicationBuilder app)
        {
            if (app is null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<CustomLogMiddleware>();
        }

        /// <summary>
        /// use时不传参
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseBrowserFilter(this IApplicationBuilder app)
        {
            return app.UseMiddleware<BrowserFilterMiddleware>();//不传参
        }

        /// <summary>
        /// use时传参，中间件构造函数中没传递参数的才会通过IOC初始化，传递了参数的以传递的为准
        /// </summary>
        /// <param name="app"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseBrowserFilter(this IApplicationBuilder app, BrowserFilterOptions options)
        {
            //非IMiddleware接口的中间件use时可以自定义传参 Middleware构造函数中没有传递的参数就会从IOC容器中去获取
            return app.UseMiddleware<BrowserFilterMiddleware>(Microsoft.Extensions.Options.Options.Create(options));//IOptions<TOptions>---OptionsWrapper----直接写入对象，直接覆盖数据-----上面那个，则是不管，以Add为准(IOption则是IOC注入的)

        }

        /// <summary>
        /// optionsAction 这个委托里面的内容会直接影响中间件BrowserFilterMiddleware 构造函数参数注入取值 参数先以use为准
        /// </summary>
        /// <param name="app"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseBrowserFilter(this IApplicationBuilder app, Action<BrowserFilterOptions> optionsAction = null)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            var browserFilterOptions = new BrowserFilterOptions();//全false
            optionsAction?.Invoke(browserFilterOptions);
            ////这里可以通过BrowserFilterMiddleware中间件的构造函数注入IOptions<BrowserFilterOptions>或IOptionsMonitor<BrowserFilterOptions>来实现
            //var configOptions = app.ApplicationServices.GetRequiredService<IConfigureOptions<BrowserFilterOptions>>();
            //configOptions.Configure(browserFilterOptions);//执行这个后就再会执行add注册BrowserFilterOptions的最后一个委托

            return app.UseBrowserFilter();
            //return app.UseBrowserFilter(browserFilterOptions);
        }


        /// <summary>
        /// UseStaticPage
        /// </summary>
        /// <param name="app"></param>
        /// <param name="directoryPath">文件写入地址,文件夹目录</param>
        /// <param name="supportDelete">是否支持删除</param>
        /// <param name="supportClear">是否支持全量删除</param>
        /// <returns></returns>
        public static IApplicationBuilder UseStaticPage(this IApplicationBuilder app, string directoryPath, bool supportDelete, bool supportClear)
        {
            return app.UseMiddleware<StaticPageMiddleware>(directoryPath, supportDelete, supportClear);
        }
    }
}
