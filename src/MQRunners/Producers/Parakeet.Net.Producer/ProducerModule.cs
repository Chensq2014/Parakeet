using Common.Storage;
using Parakeet.Net.EntityFrameworkCore;
using Serilog;
using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Parakeet.Net.Producer
{
    /// <summary>
    /// 公共生产者模块 引用NetCoreEntityFrameworkCoreModule 是为了使用EFCore Repository
    /// </summary>
    [DependsOn(typeof(NetHttpApiModule), 
        typeof(NetEntityFrameworkCoreModule),
        typeof(AbpAspNetCoreModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpAutofacModule))]
    public class ProducerModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ProducerModule)} Start  ConfigureServices ....");
            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ProducerModule)} End  ConfigureServices ....");
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ProducerModule)} Start  OnApplicationInitialization ....");
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            var configuration = context.GetConfiguration();
            //AsyncHelper.RunSync(async () =>
            //{
            //    //缓存预热
            //    //await context.ServiceProvider.WarmUpDevice();
            //    //await context.ServiceProvider.WarmUpPacketHandler();
            //    await Task.CompletedTask;
            //});
            base.OnApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ProducerModule)} End  OnApplicationInitialization ....");
        }
    }
}
