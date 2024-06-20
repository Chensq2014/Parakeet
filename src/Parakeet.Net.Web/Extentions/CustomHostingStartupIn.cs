using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Parakeet.Net.Web.Extentions;
using System;

//[assembly: HostingStartup(typeof(CustomHostingStartupIn))]
namespace Parakeet.Net.Web.Extentions
{
    /// <summary>
    /// 启动运行流程扩展
    /// </summary>
    public class CustomHostingStartupIn : IHostingStartup
    {
        public CustomHostingStartupIn()
        {
            Console.WriteLine($"********This is {nameof(CustomHostingStartupIn)} ctor********");
        }

        public void Configure(IWebHostBuilder builder)
        {
            Console.WriteLine($"********This is {nameof(CustomHostingStartupIn)} Configure********");

            #region 有IWebHostBuilder，一切都可以做。。

            builder.ConfigureAppConfiguration(configurationBuilder =>
            {
                configurationBuilder.AddXmlFile("appsettings.xml", optional: true, reloadOnChange: true);
            });//配置文件

            //builder.ConfigureServices(services =>
            //{
            //    services.TryAddScoped<IHandlerType, HandlerType>();
            //});//IOC注册


            //中间件 IWebHostBuilder的Configure只会最后一次生效 由于CustomHostingStartupOut晚创建，
            //所以CustomHostingStartupOut里的有效
            builder.Configure(app =>
            {
                app.Use(next =>
                {
                    Console.WriteLine($"This is {nameof(CustomHostingStartupIn)}-Middleware  Init");
                    return new RequestDelegate(
                        async context =>
                        {
                            Console.WriteLine($"This is {nameof(CustomHostingStartupIn)}-Middleware start");
                            await next.Invoke(context);//先In后Out? 不执行后面的中间件
                            Console.WriteLine($"This is {nameof(CustomHostingStartupIn)}-Middleware end");
                        });
                });
            });

            #endregion
        }
    }
}
