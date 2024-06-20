using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parakeet.Net.Consumer.Standard.GateModule.Forwards;
using Parakeet.Net.Consumer.Standard.Interfaces;
using Common.RabbitMQModule.Core;
using Common.Storage;
using Polly;
using Serilog;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Parakeet.Net.Consumer.Standard.GateModule
{
    /// <summary>
    /// Standard考勤模块
    /// </summary>
    [DependsOn(typeof(StandardModule))]
    public class GateModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GateModule)} Start  ConfigureServices ....");
            var config = context.Services.GetConfiguration();

            #region HttpFactory配置

            context.Services.AddHttpClient(nameof(GateModule), c =>
                {
                    c.DefaultRequestHeaders.Add("Accept", "application/json");
                })
                .AddPolicyHandler(request => Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(config.GetValue<int>("ForwardConfig:TimeOut"))))
                .AddTransientHttpErrorPolicy(builder => builder.RetryAsync(config.GetValue<int>("ForwardConfig:RetryCount")));

            #endregion HttpFactory配置

            #region IHttpForward处理注册

            context.Services.AddSingleton<IGateRecordHttpForward, GateRecordHttpForward>();

            #endregion IHttpForward处理注册

            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GateModule)} End  ConfigureServices ....");
        }
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GateModule)} Start  OnApplicationInitialization ....");
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            var configuration = context.GetConfiguration();

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、 获取IRabbitMQEventBusContainer 然后AutoRegister 自动注册并绑定当前模块的生产/消费者....");
            var rabbitEventBusContainer = context.ServiceProvider.GetService<IRabbitMQEventBusContainer>();
            rabbitEventBusContainer.AutoRegister(new[] { typeof(GateModule).Assembly });

            base.OnApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GateModule)} End  OnApplicationInitialization ....");
        }
    }
}
