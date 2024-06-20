using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Parakeet.Net.Aop;
using Parakeet.Net.GrpcService.Services;
using Common.Storage;
using Serilog;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace Parakeet.Net.GrpcService
{
    [DependsOn(typeof(AbpAspNetCoreMvcModule),typeof(NetApplicationModule))]
    public class GrpcServiceModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcServiceModule)} Start PreConfigureServices ....");
            base.PreConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcServiceModule)} End PreConfigureServices ....");
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcServiceModule)} Start ConfigureServices ....");
            //添加grpc配置
            context.Services.AddGrpc(options =>
            {
                options.Interceptors.Add<CustomServerLoggerInterceptor>();

                options.MaxReceiveMessageSize = 6 * 1024 * 1024; // 6 MB
                options.MaxSendMessageSize = 6 * 1024 * 1024;    // 6 MB
            });
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcServiceModule)} End ConfigureServices ....");
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcServiceModule)} Start OnApplicationInitialization ....");
            var app = context.GetApplicationBuilder();

            //var configuration = context.GetConfiguration(); //env.GetAppConfiguration();
            if (context.GetEnvironment().IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                //使用grpc服务需要在这里设置中间件终结点
                endpoints.MapGrpcService<GreeterService>();
                endpoints.MapGrpcService<CustomMathService>();

                endpoints.MapGet("/", async ctx =>
                {
                    await ctx.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcServiceModule)} End OnApplicationInitialization ....");
        }

    }
}
