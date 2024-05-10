using Common;
using Common.Storage;
using Microsoft.AspNetCore.Builder;
using Serilog;
using Volo.Abp;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;

namespace Parakeet.Net;

[DependsOn(
    typeof(NetDomainModule),
    typeof(AbpMultiTenancyModule),
    typeof(AbpAspNetCoreMultiTenancyModule)
)]
public class NetMultiTenancyModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetMultiTenancyModule)} Start PreConfigureServices ....");
        base.PreConfigureServices(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetMultiTenancyModule)} End PreConfigureServices ....");
    }
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetMultiTenancyModule)} Start ConfigureServices ....");

        Configure<AbpMultiTenancyOptions>(options =>
        {
            Log.Debug($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(AbpMultiTenancyOptions)}....");
            options.IsEnabled = CommonConsts.MultiTenancyEnabled;
        });
        //Configure<AbpAspNetCoreMultiTenancyOptions>(options =>
        //{
        //    options.TenantKey = CommonConsts.TenantKey;//默认 就是 __tenantId
        //});
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetMultiTenancyModule)} Start PostConfigureServices ....");
        base.PostConfigureServices(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetMultiTenancyModule)} End PostConfigureServices ....");
    }

    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetMultiTenancyModule)} Start OnPreApplicationInitialization ....");

        base.OnPreApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetMultiTenancyModule)} End OnPreApplicationInitialization ....");
    }
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetMultiTenancyModule)} Start OnApplicationInitialization ....");
        var app = context.GetApplicationBuilder();

        ////多租户 放在启动模块
        //if (CommonConsts.MultiTenancyEnabled)
        //{
        //    //app.UseCustomMultiTenancy();
        //    app.UseMultiTenancy();
        //}
        base.OnApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetMultiTenancyModule)} End OnApplicationInitialization ....");
    }

    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetMultiTenancyModule)} Start OnPostApplicationInitialization ....");
        base.OnPostApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetMultiTenancyModule)} End OnPostApplicationInitialization ....");
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetMultiTenancyModule)} Start OnApplicationShutdown ....");
        base.OnApplicationShutdown(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetMultiTenancyModule)} End OnApplicationShutdown ....");
    }

}
