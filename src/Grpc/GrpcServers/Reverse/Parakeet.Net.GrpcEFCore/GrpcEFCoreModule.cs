using Common.RabbitMQModule;
using Common.RabbitMQModule.Core;
using Common.Storage;
using Microsoft.Extensions.DependencyInjection;
using Parakeet.Net.EntityFrameworkCore;
using Parakeet.Net.Equipment;
using Serilog;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Parakeet.Net.GrpcEFCore
{
    /// <summary>
    /// 整一个可以efcore 读写数据库的 公共module 提供给上层模块使用
    /// </summary>
    [DependsOn(typeof(NetEntityFrameworkCoreModule),
        typeof(EquipmentModule),
        typeof(CustomRabbitMQModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpAutofacModule)
    )]
    public class GrpcEFCoreModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcEFCoreModule)} Start PreConfigureServices ....");
            base.PreConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcEFCoreModule)} End PreConfigureServices ....");

        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcEFCoreModule)} Start ConfigureServices ....");
            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcEFCoreModule)} End ConfigureServices ....");
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcEFCoreModule)} Start PostConfigureServices ....");
            base.PostConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcEFCoreModule)} End PostConfigureServices ....");
        }

        public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcEFCoreModule)} Start OnPreApplicationInitialization ....");
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            var configuration = context.GetConfiguration();
            base.OnPreApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcEFCoreModule)} End OnPreApplicationInitialization ....");
        }
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcEFCoreModule)} Start OnApplicationInitialization ....");

            var rabbitEventBusContainer = context.ServiceProvider.GetService<IRabbitMQEventBusContainer>();
            rabbitEventBusContainer.AutoRegister(new[] { typeof(GrpcEFCoreModule).Assembly });
            base.OnApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcEFCoreModule)} End OnApplicationInitialization ....");
        }

        public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcEFCoreModule)} Start OnPostApplicationInitialization ....");
            base.OnPostApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcEFCoreModule)} End OnPostApplicationInitialization ....");
        }

        public override void OnApplicationShutdown(ApplicationShutdownContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcEFCoreModule)} Start OnApplicationShutdown ....");
            base.OnApplicationShutdown(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcEFCoreModule)} End OnApplicationShutdown ....");
        }
    }
}
