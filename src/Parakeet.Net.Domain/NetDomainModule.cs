using AutoMapper.Internal;
using Common;
using Common.CacheMudule;
using Common.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Parakeet.Net.Settings;
using Serilog;
using System.Threading;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Emailing;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.PermissionManagement.OpenIddict;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Volo.Abp.TenantManagement;

namespace Parakeet.Net;

[DependsOn(
    typeof(NetCacheMudule),
    typeof(CommonEntityModule),
    typeof(NetDomainSharedModule),
    typeof(AbpAuditLoggingDomainModule),
    typeof(AbpBackgroundJobsDomainModule),
    typeof(AbpFeatureManagementDomainModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpOpenIddictDomainModule),
    typeof(AbpPermissionManagementDomainOpenIddictModule),
    typeof(AbpPermissionManagementDomainIdentityModule),
    typeof(AbpSettingManagementDomainModule),
    typeof(AbpTenantManagementDomainModule),
    typeof(AbpEmailingModule)
)]
public class NetDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ////NetDomainSharedModule模块已添加
        //Configure<AbpLocalizationOptions>(options =>
        //{
        //    options.Languages.Add(new LanguageInfo("en", "en", "English", "gb"));
        //    options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
        //});

        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = CommonConsts.MultiTenancyEnabled;
        });

        Log.Debug($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置EmailSender->{nameof(NullEmailSender)}....");
        context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());

        //默认配置是不合适，如果需要自己配置，则需要在选择合适SettingProvider，在官方提供了五种方法，
        //它使用的倒序的做的法，先User>Tenant>Global>Configuration>Default

        Configure<AbpSettingOptions>(options =>
        {
            Log.Error($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(AbpSettingOptions)} ....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            //优先级顺序6-1   回退系统从底部 (用户) 到 (自定义) 方向起用用. 6最后执行，所以同配置的6优先
            options.ValueProviders.TryAdd<CustomSettingValueProvider>();//自定义设置值提供程序 6
            options.ValueProviders.TryAdd<DefaultValueSettingValueProvider>();//从设置定义的默认值中获取值 5
            options.ValueProviders.TryAdd<ConfigurationSettingValueProvider>();//从IConfiguration服务中获取值 4 定义了配置文件Settings:节点
            options.ValueProviders.TryAdd<GlobalSettingValueProvider>();//获取设置的全局(系统范围)值. 3
            options.ValueProviders.TryAdd<TenantSettingValueProvider>();//获取当前租户的设置值 2
            options.ValueProviders.TryAdd<UserSettingValueProvider>();//获取当前用户的设置值 1

            ////可以不用写 系统自动发现define
            //options.DefinitionProviders.Add<EmailSettingProvider>();
            //options.DefinitionProviders.Add<NetSettingDefinitionProvider>();
        });
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainModule)} End ConfigureServices ....");


    }
}
