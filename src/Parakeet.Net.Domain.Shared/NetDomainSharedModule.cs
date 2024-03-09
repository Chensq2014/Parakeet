using System.Threading;
using Common;
using Common.Storage;
using Parakeet.Net.Localization;
using Serilog;
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
    typeof(AbpTenantManagementDomainSharedModule),
    typeof(CommonSharedModule)  
    )]
public class NetDomainSharedModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        Log.Error($"{{0}}", $"............................................................................................................................");
        Log.Error($"{{0}}", $"..............................................PreConfigureServices..........................................................");
        Log.Error($"{{0}}", $"............................................................................................................................");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainSharedModule)} Start PreConfigureServices ....");
        NetGlobalFeatureConfigurator.Configure();
        NetModuleExtensionConfigurator.Configure();
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainSharedModule)} End PreConfigureServices ....");
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Log.Error($"{{0}}", $"............................................................................................................................");
        Log.Error($"{{0}}", $"..............................................ConfigureServices.............................................................");
        Log.Error($"{{0}}", $"............................................................................................................................");

        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainSharedModule)} Start ConfigureServices ....");
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            Log.Error($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(AbpVirtualFileSystemOptions)}添加嵌入资源到虚拟文件系统 ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            options.FileSets.AddEmbedded<NetDomainSharedModule>();//"Parakeet.Net"
            //options.FileSets.Add(new EmbeddedFileSet(typeof(NetDomainSharedModule).Assembly));
            Log.Error($"{{0}}", $"{CacheKeys.LogCount++}、添加嵌入资源到虚拟文件系统完毕....");
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            Log.Error($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(AbpLocalizationOptions)}默认本地化为zh-Hans(中文),及添加资源到虚拟文件系统...ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            options.Resources
                .Add<NetResource>("zh-Hans")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/Net");

            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
            options.DefaultResourceType = typeof(NetResource);
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            Log.Error($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(AbpExceptionLocalizationOptions)}...ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            options.MapCodeNamespace("Net", typeof(NetResource));
        });
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainSharedModule)} End ConfigureServices ....");
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        Log.Error($"{{0}}", $"............................................................................................................................");
        Log.Error($"{{0}}", $"..............................................PostConfigureServices.........................................................");
        Log.Error($"{{0}}", $"............................................................................................................................");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainSharedModule)} Start PostConfigureServices ....");
        base.PostConfigureServices(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainSharedModule)} End PostConfigureServices ....");
    }
}
