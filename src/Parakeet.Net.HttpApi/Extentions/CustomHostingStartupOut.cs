using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Parakeet.Net.Exetentions;
using System;

//[assembly: HostingStartup(typeof(CustomHostingStartupOut))]
namespace Parakeet.Net.Exetentions
{
    /// <summary>
    /// 启动运行流程扩展Out
    /// </summary>
    public class CustomHostingStartupOut : IHostingStartup
    {
        public CustomHostingStartupOut()
        {
            Console.WriteLine($"********This is {nameof(CustomHostingStartupOut)} ctor********");
        }

        public void Configure(IWebHostBuilder builder)
        {
            Console.WriteLine($"********This is {nameof(CustomHostingStartupOut)} Configure********");

            #region 有IWebHostBuilder，一切都可以做。。

            //builder.ConfigureAppConfiguration(configurationBuilder =>
            //{
            //    configurationBuilder.AddXmlFile("appsettings.xml", optional: true, reloadOnChange: true);
            //});//配置文件

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
                    Console.WriteLine($"This is {nameof(CustomHostingStartupOut)}-Middleware  Init");
                    return new RequestDelegate(
                        async context =>
                        {
                            Console.WriteLine($"This is {nameof(CustomHostingStartupOut)}-Middleware start");
                            await next.Invoke(context);//先In后Out 不执行后面的
                            Console.WriteLine($"This is {nameof(CustomHostingStartupOut)}-Middleware end");
                        });
                });
            });

            #endregion
        }
    }
}
