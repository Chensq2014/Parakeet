using Microsoft.Extensions.DependencyInjection;
using Common.RabbitMQModule.Core;
using Common.Storage;
using Serilog;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Parakeet.Net.Producer.Chongqing
{
    [DependsOn(typeof(ProducerModule))]
    public class ChongqingModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(ChongqingModule)} Start  PreConfigureServices ....");
            //PreConfigure<IMvcBuilder>(mvcBuilder =>
            //{
            //    mvcBuilder.AddApplicationPartIfNotExists(typeof(ChongqingModule).Assembly);
            //});
            base.PreConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(ChongqingModule)} End  PreConfigureServices ....");
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(ChongqingModule)} Start  ConfigureServices ....");
            //var configuration = context.Services.GetConfiguration();
            //context.Services.AddHttpApi<IImageApi>(options =>
            //{
            //    options.HttpHost = new Uri(configuration.GetValue<string>("HttpOptions:ImageUrl"));
            //});
            //context.Services.BuildServiceProvider();
            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(ChongqingModule)} End  ConfigureServices ....");
        }



        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(ChongqingModule)} Start  OnApplicationInitialization ....");
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            var configuration = context.GetConfiguration();
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}�� ��ȡIRabbitMQEventBusContainer Ȼ��AutoRegister �Զ�ע�Ტ�󶨵�ǰģ�������/������....");
            var rabbitEventBusContainer = context.ServiceProvider.GetService<IRabbitMQEventBusContainer>();
            rabbitEventBusContainer.AutoRegister(new[] { typeof(ChongqingModule).Assembly });

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(ChongqingModule)} End  OnApplicationInitialization ....");
        }

    }
}
