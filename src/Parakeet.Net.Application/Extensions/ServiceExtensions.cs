using Common.Dtos;
using Common.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Parakeet.Net.Middleware;
using Parakeet.Net.Options;
using System;

namespace Common.Extensions
{
    /// <summary>
    /// 服务注册扩展
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 替换注册中间件工厂
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static void AddMiddlewareFactory(this IServiceCollection services)
        {
            services.Replace(ServiceDescriptor.Singleton<IMiddlewareFactory, ParakeetMiddlewareFactory>());//替换默认中间件容器
        }

        /// <summary>
        /// 替换注册中间件工厂
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static void AddInheritedMiddleware(this IServiceCollection services)
        {
            services.AddSingleton<InheritedMiddleware>();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static void AddBrowserFilter(this IServiceCollection services)
        {
            services.TryAddSingleton<IBrowserCheck, BrowserCheckService>();
        }

        /// <summary>
        /// 直接用Option的模式去初始化
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void AddBrowserFilter(this IServiceCollection services, Action<BrowserFilterOptions> configure)
        {
            services.Configure(configure);//这个是Options,只是增加单例委托配置项，生效是在访问Value属性时 通过委托一步一步组装的同一个单例BrowserFilterOptions
            //等同于以下代码
            //services.Configure<BrowserFilterOptions>(configure);
            //services.Configure<BrowserFilterOptions>(option =>
            //{
            //    //add之后,再通过use这个委托再配置一遍 BrowserFilterOptions
            //    option.EnableEdge =true;
            //    option.EnableChorme = true;
            //    option.EnableFirefox = true;
            //    option.EnableIE =true;
            //});
            services.AddBrowserFilter();
        }

    }
}
