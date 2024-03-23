using Common.Interfaces;
using Common.RabbitMQModule.Core;
using Common.RabbitMQModule.Producers;
using Common.Storage;
using Microsoft.Extensions.DependencyInjection;
using Parakeet.Net.Equipment;
using Parakeet.Net.GrpcEFCore;
using Serilog;
using System.Threading;
using Volo.Abp;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Parakeet.Net.Register
{
    [DependsOn(typeof(GrpcEFCoreModule))]
    public class EquipmentRegisterModule: AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentRegisterModule)} Start PreConfigureServices ....");
            base.PreConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentRegisterModule)} End PreConfigureServices ....");

        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentRegisterModule)} Start ConfigureServices ....");

            #region 重庆 Standard

            context.Services.AddScoped<IReverseCommand, Commands.Chongqing.PersonRegisterCommand>();
            context.Services.AddScoped<IReverseCommand, Commands.Standard.PersonRegisterCommand>();

            #endregion 重庆 Standard


            #region AutoMapper

            //添加对AutoMapper的支持
            //context.Services.AddAutoMapper();
            Configure<AbpAutoMapperOptions>(options =>
            {
                Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(EquipmentRegisterModule)} AbpAutoMapperOptions...ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                options.AddMaps<EquipmentRegisterModule>();//options.AddProfile<RegisterMapperProfile>();
            });

            #endregion

            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentRegisterModule)} End ConfigureServices ....");
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentRegisterModule)} Start PostConfigureServices ....");
            base.PostConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentRegisterModule)} End PostConfigureServices ....");
        }

        public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentRegisterModule)} Start OnPreApplicationInitialization ....");

            var eventBusContainer = context.ServiceProvider.GetRequiredService<IRabbitMQEventBusContainer>();
            eventBusContainer.AddProducer(new ProducerAttribute(EquipmentConstants.STANDARD, EquipmentConstants.GATE_EXCHANGE));
            eventBusContainer.AddProducer(new ProducerAttribute(EquipmentConstants.SICHUAN, EquipmentConstants.GATE_EXCHANGE));
            eventBusContainer.AddProducer(new ProducerAttribute(EquipmentConstants.CHONGQING, EquipmentConstants.GATE_EXCHANGE));
            eventBusContainer.AddProducer(new ProducerAttribute(EquipmentConstants.HUNAN, EquipmentConstants.GATE_EXCHANGE));
            base.OnPreApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentRegisterModule)} End OnPreApplicationInitialization ....");
        }
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentRegisterModule)} Start OnApplicationInitialization ....");
            var rabbitEventBusContainer = context.ServiceProvider.GetService<IRabbitMQEventBusContainer>();
            rabbitEventBusContainer.AutoRegister(new[] { typeof(EquipmentRegisterModule).Assembly });
            base.OnApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentRegisterModule)} End OnApplicationInitialization ....");
        }

        public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentRegisterModule)} Start OnPostApplicationInitialization ....");
            base.OnPostApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentRegisterModule)} End OnPostApplicationInitialization ....");
        }

        public override void OnApplicationShutdown(ApplicationShutdownContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentRegisterModule)} Start OnApplicationShutdown ....");
            base.OnApplicationShutdown(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EquipmentRegisterModule)} End OnApplicationShutdown ....");
        }
    }
}
