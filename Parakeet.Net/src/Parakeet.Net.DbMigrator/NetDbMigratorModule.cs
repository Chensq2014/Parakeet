using Parakeet.Net.Cache;
using Parakeet.Net.EntityFrameworkCore;
using Serilog;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace Parakeet.Net.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(NetEntityFrameworkCoreModule),
    typeof(NetApplicationContractsModule)
    )]
public class NetDbMigratorModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDbMigratorModule)} Start PreConfigureServices ....");
        base.PreConfigureServices(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDbMigratorModule)} End PreConfigureServices ....");
    }





    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDbMigratorModule)} Start ConfigureServices ....");

        Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);

        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDbMigratorModule)} End ConfigureServices ....");

    }




    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDbMigratorModule)} Start PostConfigureServices ....");
        base.PostConfigureServices(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDbMigratorModule)} End PostConfigureServices ....");
    }
    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDbMigratorModule)} Start OnPreApplicationInitialization ....");
        base.OnPreApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDbMigratorModule)} End OnPreApplicationInitialization ....");
    }
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDbMigratorModule)} Start OnApplicationInitialization ....");
        base.OnApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDbMigratorModule)} End OnApplicationInitialization ....");
    }

    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDbMigratorModule)} Start OnPostApplicationInitialization ....");
        base.OnPostApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDbMigratorModule)} End OnPostApplicationInitialization ....");
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDbMigratorModule)} Start OnApplicationShutdown ....");
        base.OnApplicationShutdown(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDbMigratorModule)} End OnApplicationShutdown ....");
    }


}
