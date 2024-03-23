using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parakeet.Net.Consumer.Chongqing.Interfaces;
using Parakeet.Net.Consumer.Chongqing.RegisterModule.Forwards;
using Common.RabbitMQModule.Core;
using Common.Storage;
using Polly;
using Serilog;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Parakeet.Net.Consumer.Chongqing.RegisterModule
{
    /// <summary>
    /// Chongqing注册模块
    /// </summary>
    [DependsOn(typeof(ChongqingModule))]
    public class RegisterModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(RegisterModule)} Start  ConfigureServices ....");

            var config = context.Services.GetConfiguration();
            #region HttpFactory配置

            context.Services.AddHttpClient(nameof(RegisterModule), c =>
                {
                    c.DefaultRequestHeaders.Add("Accept", "application/json");
                })
                .AddPolicyHandler(request => Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(config.GetValue<int>("ForwardConfig:TimeOut"))))
                .AddTransientHttpErrorPolicy(builder => builder.RetryAsync(config.GetValue<int>("ForwardConfig:RetryCount")));

            #endregion HttpFactory配置

            #region IHttpForward处理注册

            context.Services.AddTransient<IPersonRegisterHttpForward, PersonRegisterHttpForward>();

            #endregion IHttpForward处理注册
            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(RegisterModule)} End  ConfigureServices ....");
        }
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(RegisterModule)} Start  OnApplicationInitialization ....");
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            var configuration = context.GetConfiguration();

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、 获取IRabbitMQEventBusContainer 然后AutoRegister 自动注册并绑定当前模块的生产/消费者....");
            var rabbitEventBusContainer = context.ServiceProvider.GetService<IRabbitMQEventBusContainer>();
            rabbitEventBusContainer.AutoRegister(new[] { typeof(RegisterModule).Assembly });

            base.OnApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(RegisterModule)} End  OnApplicationInitialization ....");
        }
    }
}
