using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Parakeet.Net.GrpcService.Services;
using System;
using Microsoft.Extensions.Logging;
using Serilog;
using Volo.Abp;

namespace Parakeet.Net.GrpcService
{
    public class Startup
    {
        //private IAbpApplicationWithExternalServiceProvider _abpApp;
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger.Information($"{nameof(Startup)} Start ConfigureServices ....");
            //_abpApp = services.AddApplication<GrpcServiceModule>();
            services.AddApplication<GrpcServiceModule>();
            Log.Logger.Information($"{nameof(Startup)} End ConfigureServices ....");
        }

        /// <summary>
        /// 配置管道
        /// 基于IApplicationBuilderFactory创建IApplicationBuilder对象
        /// 基于IStartupFilter的管道构建
        /// 调用IApplicationBuilder对象的Build方法完成完整的管道
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            ILogger<Startup> logger = loggerFactory.CreateLogger<Startup>();
            logger.LogDebug($"{nameof(Startup)}默认配置管道开始.....");
            //_abpApp.Initialize(app.ApplicationServices);
            app.InitializeApplication();
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            //app.UseRouting();

            //app.UseEndpoints(endpoints =>
            //{
            //    //使用grpc服务需要在这里设置中间件终结点
            //    endpoints.MapGrpcService<GreeterService>();
            //    endpoints.MapGrpcService<CustomMathService>();

            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            //    });
            //});

            Log.Logger.Information($"{nameof(Startup)}默认配置管道结束.....");
        }
    }
}
