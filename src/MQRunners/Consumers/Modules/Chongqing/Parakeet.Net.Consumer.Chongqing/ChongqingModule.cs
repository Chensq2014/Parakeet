using Microsoft.Extensions.DependencyInjection;
using Parakeet.Net.Consumer;
using Parakeet.Net.Consumer.Chongqing.Dtos;
using Common.Storage;
using Serilog;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Parakeet.Net.Consumer.Chongqing
{
    /// <summary>
    /// Chongqing(重庆)区域公共模块
    /// </summary>
    [DependsOn(typeof(ConsumerModule))]
    public class ChongqingModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ChongqingModule)} Start  ConfigureServices ....");

            var configuration = context.Services.GetConfiguration();
            context.Services.Configure<TokenConfigDto>(configuration.GetSection("ForwardBaseAddress:ChongqingTokenConfig"));

            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ChongqingModule)} End  ConfigureServices ....");
        }
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ChongqingModule)} Start  OnApplicationInitialization ....");
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            var configuration = context.GetConfiguration();

            base.OnApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ChongqingModule)} End  OnApplicationInitialization ....");
        }
    }
}
