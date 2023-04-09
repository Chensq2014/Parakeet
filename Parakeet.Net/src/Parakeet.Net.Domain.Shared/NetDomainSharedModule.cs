using System.Threading;
using Parakeet.Net.Cache;
using Parakeet.Net.Localization;
using Serilog;
using Volo.Abp;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Parakeet.Net;

[DependsOn(
    typeof(AbpAuditLoggingDomainSharedModule),
    typeof(AbpBackgroundJobsDomainSharedModule),
    typeof(AbpFeatureManagementDomainSharedModule),
    typeof(AbpIdentityDomainSharedModule),
    typeof(AbpOpenIddictDomainSharedModule),
    typeof(AbpPermissionManagementDomainSharedModule),
    typeof(AbpSettingManagementDomainSharedModule),
    typeof(AbpTenantManagementDomainSharedModule)    
    )]
public class NetDomainSharedModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        Log.Debug($"{{0}}", $"............................................................................................................................");
        Log.Debug($"{{0}}", $"..............................................PreConfigureServices..........................................................");
        Log.Debug($"{{0}}", $"............................................................................................................................");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainSharedModule)} Start PreConfigureServices ....");
        NetGlobalFeatureConfigurator.Configure();
        NetModuleExtensionConfigurator.Configure(); 
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainSharedModule)} End PreConfigureServices ....");

    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Log.Debug($"{{0}}", $"............................................................................................................................");
        Log.Debug($"{{0}}", $"..............................................ConfigureServices.............................................................");
        Log.Debug($"{{0}}", $"............................................................................................................................");

        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainSharedModule)} Start ConfigureServices ....");

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            Log.Debug($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(AbpVirtualFileSystemOptions)}添加嵌入资源到虚拟文件系统 ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");

            options.FileSets.AddEmbedded<NetDomainSharedModule>("Parakeet.Net");
            //options.FileSets.Add(new EmbeddedFileSet(typeof(NetDomainSharedModule).Assembly));
            Log.Debug($"{{0}}", $"{CacheKeys.LogCount++}、添加嵌入资源到虚拟文件系统完毕....");

        });

        Configure<AbpLocalizationOptions>(options =>
        {
            Log.Debug($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(AbpLocalizationOptions)}默认本地化为zh-Hans(中文),及添加资源到虚拟文件系统...ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            options.Resources
                .Add<NetResource>("zh-Hans") //添加本地化资源NetCoreResource, 默认本地化为"zh-Hans"(中文).
                .AddBaseTypes(typeof(AbpValidationResource))//从已有资源文件继承 如果扩展文件定义了相同的本地化字符串, 那么它会覆盖该字符串
                //.AddBaseTypes(typeof(AbpUiResource))
                .AddVirtualJson("/Localization/Net");//用JSON文件存储本地化字符串.使用虚拟文件系统 将JSON文件嵌入到程序集中.
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
            options.DefaultResourceType = typeof(NetResource);
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            Log.Debug($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(AbpExceptionLocalizationOptions)}...ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");

            options.MapCodeNamespace("Net", typeof(NetResource));
        });
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainSharedModule)} End ConfigureServices ....");

    }


    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        Log.Debug($"{{0}}", $"............................................................................................................................");
        Log.Debug($"{{0}}", $"..............................................PostConfigureServices.........................................................");
        Log.Debug($"{{0}}", $"............................................................................................................................");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainSharedModule)} Start PostConfigureServices ....");
        base.PostConfigureServices(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainSharedModule)} End PostConfigureServices ....");
    }

    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Debug($"{{0}}", $"............................................................................................................................");
        Log.Debug($"{{0}}", $"..............................................OnPreApplicationInitialization................................................");
        Log.Debug($"{{0}}", $"............................................................................................................................");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainSharedModule)} Start OnPreApplicationInitialization ....");
        base.OnPreApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainSharedModule)} End OnPreApplicationInitialization ....");
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Debug($"{{0}}", $"............................................................................................................................");
        Log.Debug($"{{0}}", $"..............................................OnApplicationInitialization...................................................");
        Log.Debug($"{{0}}", $"............................................................................................................................");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainSharedModule)} Start OnApplicationInitialization ....");
        base.OnApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainSharedModule)} End OnApplicationInitialization ....");
    }

    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Debug($"{{0}}", $"............................................................................................................................");
        Log.Debug($"{{0}}", $"..............................................OnPostApplicationInitialization...............................................");
        Log.Debug($"{{0}}", $"............................................................................................................................");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainSharedModule)} Start OnPostApplicationInitialization ....");
        base.OnPostApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainSharedModule)} End OnPostApplicationInitialization ....");
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainSharedModule)} Start OnApplicationShutdown ....");
        base.OnApplicationShutdown(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainSharedModule)} End OnApplicationShutdown ....");

        Log.Debug($"{{0}}", $"............................................................................................................................");
        Log.Debug($"{{0}}", $"..............................................OnApplicationShutdown 反序最后执行............................................");
        Log.Debug($"{{0}}", $"............................................................................................................................");
    }
}
