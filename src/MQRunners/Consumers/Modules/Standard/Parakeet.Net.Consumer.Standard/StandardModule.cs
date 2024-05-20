using Microsoft.Extensions.DependencyInjection;
using Parakeet.Net.Consumer.Standard.Dtos;
using Common.Storage;
using Serilog;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Parakeet.Net.Consumer.Standard
{
    /// <summary>
    /// Standard区域公共模块
    /// </summary>
    [DependsOn(typeof(ConsumerModule))]
    public class StandardModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(StandardModule)} Start  ConfigureServices ....");

            var configuration = context.Services.GetConfiguration();
            context.Services.Configure<TokenConfigDto>(configuration.GetSection("ForwardBaseAddress:StandardTokenConfig"));

            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(StandardModule)} End  ConfigureServices ....");
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(StandardModule)} Start  OnApplicationInitialization ....");
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            var configuration = context.GetConfiguration();

            base.OnApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(StandardModule)} End  OnApplicationInitialization ....");
        }
    }
}
