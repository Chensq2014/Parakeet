using Common.RabbitMQModule;
using Common.RabbitMQModule.Core;
using Common.Storage;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Parakeet.Net.Equipment
{
    [DependsOn(typeof(NetApplicationContractsModule), typeof(CustomRabbitMQModule))]
    public class EquipmentModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentModule)} Start PreConfigureServices ....");
            base.PreConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentModule)} End PreConfigureServices ....");

        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentModule)} Start ConfigureServices ....");
            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentModule)} End ConfigureServices ....");
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentModule)} Start PostConfigureServices ....");
            base.PostConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentModule)} End PostConfigureServices ....");
        }

        public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentModule)} Start OnPreApplicationInitialization ....");
            base.OnPreApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentModule)} End OnPreApplicationInitialization ....");
        }
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentModule)} Start OnApplicationInitialization ....");
            var rabbitEventBusContainer = context.ServiceProvider.GetService<IRabbitMQEventBusContainer>();
            rabbitEventBusContainer.AutoRegister(new[] { typeof(EquipmentModule).Assembly });
            base.OnApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentModule)} End OnApplicationInitialization ....");
        }

        public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentModule)} Start OnPostApplicationInitialization ....");
            base.OnPostApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentModule)} End OnPostApplicationInitialization ....");
        }

        public override void OnApplicationShutdown(ApplicationShutdownContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentModule)} Start OnApplicationShutdown ....");
            base.OnApplicationShutdown(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentModule)} End OnApplicationShutdown ....");
        }
    }
}
