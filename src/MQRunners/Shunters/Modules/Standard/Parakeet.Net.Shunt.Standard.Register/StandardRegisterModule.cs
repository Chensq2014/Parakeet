﻿using Microsoft.Extensions.DependencyInjection;
using Common.RabbitMQModule.Core;
using Common.Storage;
using Serilog;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Parakeet.Net.Shunt.Standard.Register
{
    [DependsOn(typeof(ShuntModule))]
    public class StandardRegisterModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(StandardRegisterModule)} Start  ConfigureServices ....");
            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(StandardRegisterModule)} End  ConfigureServices ....");
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(StandardRegisterModule)} Start  OnApplicationInitialization ....");
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            var configuration = context.GetConfiguration();

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、 获取IRabbitMQEventBusContainer 然后AutoRegister 自动注册并绑定当前模块的生产/消费者....");
            //自动注册当前模块下 所有的生产者和消费者
            var rabbitEventBusContainer = context.ServiceProvider.GetService<IRabbitMQEventBusContainer>();
            rabbitEventBusContainer.AutoRegister(new[] { typeof(StandardRegisterModule).Assembly });

            base.OnApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(StandardRegisterModule)} End  OnApplicationInitialization ....");
        }
    }
}