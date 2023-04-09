using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Parakeet.Net.Cache;
using Parakeet.Net.Entities;
using Parakeet.Net.MultiTenancy;
using Serilog;
using StackExchange.Redis;
using System.Threading;
using Parakeet.Net.Settings;
using Volo.Abp;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Emailing;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
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

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainModule)} Start PreConfigureServices ....");
        base.PreConfigureServices(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainModule)} End PreConfigureServices ....");
    }
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainModule)} Start ConfigureServices ....");

        Configure<AbpMultiTenancyOptions>(options =>
        {
            Log.Debug($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(AbpMultiTenancyOptions)}....");
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });

#if DEBUG 
        Log.Debug($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置EmailSender->{nameof(NullEmailSender)}....");
        context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
#endif
        
        
        //配置实体默认include项 导航属性级别: x:0 y:1 z:2 o:3 p:4 q:5( 一般默认配置1-2级导航属性)

        context.Services.Configure<AbpEntityOptions>(options =>
        {
            Log.Error($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(AbpEntityOptions)} 实体默认include项 AbpEntityOptions 导航属性级别: x:0 y:1 z:2 o:3 p:4 q:5( 一般默认配置1-2级导航属性)....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            //options.Entity<Organization>(c =>
            //{
            //    c.DefaultWithDetailsFunc = x => x
            //        .Include(y => y.LocationArea)
            //        .Include(y => y.Parent)
            //        .Include(y => y.Children)
            //        .Include(y => y.Projects)
            //        .Include(y => y.Users).ThenInclude(z => z.User);
            //});

            //options.Entity<OrganizationUser>(c =>
            //{
            //    c.DefaultWithDetailsFunc = x => x
            //        .Include(y => y.Organization).ThenInclude(z => z.LocationArea)
            //        .Include(y => y.Organization).ThenInclude(z => z.Parent)
            //        .Include(y => y.Organization).ThenInclude(z => z.Children)
            //        .Include(y => y.Organization).ThenInclude(z => z.Projects)
            //        .Include(y => y.User);
            //});

            //options.Entity<Project>(c =>
            //{
            //    c.DefaultWithDetailsFunc = x => x
            //        .Include(y => y.LocationArea)
            //        .Include(y => y.Organization).ThenInclude(z => z.LocationArea)
            //        .Include(y => y.Organization).ThenInclude(z => z.Parent)
            //        .Include(y => y.Organization).ThenInclude(z => z.Children)
            //        .Include(y => y.Organization).ThenInclude(z => z.Users).ThenInclude(o => o.User)
            //        .Include(y => y.Organization).ThenInclude(z => z.Projects)
            //        .Include(y => y.Devices).ThenInclude(z => z.Project)
            //        .Include(y => y.Devices).ThenInclude(z => z.Threshold)
            //        .Include(y => y.Devices).ThenInclude(z => z.Sequence)
            //        .Include(y => y.Devices).ThenInclude(z => z.AreaTenant)
            //        .Include(y => y.Devices).ThenInclude(z => z.LocationArea)
            //        .Include(y => y.Devices).ThenInclude(z => z.Extends)
            //        .Include(y => y.Devices).ThenInclude(z => z.Mediators)
            //        .Include(y => y.Devices).ThenInclude(z => z.DeviceWorkers)
            //        .Include(y => y.ProjectUsers).ThenInclude(z => z.User)
            //        .Include(y => y.Attachments);
            //});

            //options.Entity<ProjectUser>(c =>
            //{
            //    c.DefaultWithDetailsFunc = x => x
            //        .Include(y => y.Project).ThenInclude(z => z.LocationArea)
            //        .Include(y => y.Project).ThenInclude(z => z.Organization)
            //        .Include(y => y.Project).ThenInclude(z => z.Devices)
            //        .Include(y => y.User);
            //});

            //options.Entity<Supplier>(c =>
            //{
            //    c.DefaultWithDetailsFunc = x => x
            //        .Include(y => y.LocationArea)
            //        .Include(y => y.Devices).ThenInclude(z => z.Project).ThenInclude(o => o.Organization)
            //        .Include(y => y.Devices).ThenInclude(z => z.Threshold)
            //        .Include(y => y.Devices).ThenInclude(z => z.Sequence)
            //        .Include(y => y.Devices).ThenInclude(z => z.AreaTenant)
            //        .Include(y => y.Devices).ThenInclude(z => z.LocationArea)
            //        .Include(y => y.Devices).ThenInclude(z => z.Extends)
            //        .Include(y => y.Devices).ThenInclude(z => z.Mediators)
            //        .Include(y => y.Devices).ThenInclude(z => z.DeviceWorkers);
            //});

            //options.Entity<Threshold>(c =>
            //{
            //    c.DefaultWithDetailsFunc = x => x
            //        .Include(y => y.Devices).ThenInclude(z => z.Project)
            //        .Include(y => y.Devices).ThenInclude(z => z.Supplier)
            //        .Include(y => y.Devices).ThenInclude(z => z.Sequence)
            //        .Include(y => y.Devices).ThenInclude(z => z.AreaTenant)
            //        .Include(y => y.Devices).ThenInclude(z => z.LocationArea)
            //        .Include(y => y.Devices).ThenInclude(z => z.Extends)
            //        .Include(y => y.Devices).ThenInclude(z => z.Mediators)
            //        .Include(y => y.Devices).ThenInclude(z => z.DeviceWorkers);
            //});

            //options.Entity<Device>(c =>
            //{
            //    c.DefaultWithDetailsFunc = x => x
            //        .Include(y => y.Sequence)
            //        .Include(y => y.Threshold)
            //        .Include(y => y.LocationArea)
            //        .Include(y => y.Project).ThenInclude(z => z.LocationArea)
            //        .Include(y => y.Project).ThenInclude(z => z.Organization).ThenInclude(o => o.LocationArea)
            //        .Include(y => y.Project).ThenInclude(z => z.Organization).ThenInclude(o => o.Parent)
            //        .Include(y => y.Project).ThenInclude(z => z.Organization).ThenInclude(o => o.Children)
            //        .Include(y => y.Project).ThenInclude(z => z.Organization).ThenInclude(o => o.Projects)
            //        .Include(y => y.Project).ThenInclude(z => z.Organization).ThenInclude(o => o.Users)
            //        .Include(y => y.Project).ThenInclude(z => z.ProjectUsers).ThenInclude(o => o.User)
            //        .Include(y => y.Project).ThenInclude(z => z.Attachments)
            //        .Include(y => y.Supplier).ThenInclude(z => z.LocationArea)
            //        .Include(y => y.AreaTenant).ThenInclude(z => z.LocationArea)
            //        .Include(y => y.AreaTenant).ThenInclude(z => z.TenantDbConnectionStrings)
            //        .Include(y => y.AreaTenant).ThenInclude(z => z.Devices)
            //        .Include(y => y.DeviceWorkers).ThenInclude(z => z.Worker)
            //        .Include(y => y.Mediators).ThenInclude(z => z.Mediator)
            //        .Include(y => y.Extends)
            //        .Include(y => y.KeySecrets);
            //});

            //options.Entity<DeviceExtend>(c =>
            //{
            //    c.DefaultWithDetailsFunc = x => x
            //        .Include(y => y.Device).ThenInclude(z => z.Supplier).ThenInclude(o => o.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.Project).ThenInclude(o => o.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.Project).ThenInclude(o => o.Organization).ThenInclude(p => p.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.Project).ThenInclude(o => o.Organization).ThenInclude(p => p.Parent)
            //        .Include(y => y.Device).ThenInclude(z => z.Project).ThenInclude(o => o.Organization).ThenInclude(p => p.Children)
            //        .Include(y => y.Device).ThenInclude(z => z.AreaTenant).ThenInclude(o => o.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.Threshold)
            //        .Include(y => y.Device).ThenInclude(z => z.Sequence)
            //        .Include(y => y.Device).ThenInclude(z => z.Extends)
            //        .Include(y => y.Device).ThenInclude(z => z.Mediators)
            //        .Include(y => y.Device).ThenInclude(z => z.KeySecrets)
            //        .Include(y => y.Device).ThenInclude(z => z.DeviceWorkers);
            //});
            //options.Entity<DeviceKeySecret>(c =>
            //{
            //    c.DefaultWithDetailsFunc = x => x
            //        .Include(y => y.Device).ThenInclude(z => z.Supplier).ThenInclude(o => o.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.Project).ThenInclude(o => o.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.Project).ThenInclude(o => o.Organization).ThenInclude(p => p.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.Project).ThenInclude(o => o.Organization).ThenInclude(p => p.Parent)
            //        .Include(y => y.Device).ThenInclude(z => z.Project).ThenInclude(o => o.Organization).ThenInclude(p => p.Children)
            //        .Include(y => y.Device).ThenInclude(z => z.AreaTenant).ThenInclude(o => o.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.Threshold)
            //        .Include(y => y.Device).ThenInclude(z => z.Sequence)
            //        .Include(y => y.Device).ThenInclude(z => z.Extends)
            //        .Include(y => y.Device).ThenInclude(z => z.Mediators)
            //        .Include(y => y.Device).ThenInclude(z => z.KeySecrets)
            //        .Include(y => y.Device).ThenInclude(z => z.DeviceWorkers);
            //});

            //options.Entity<DeviceSequence>(c =>
            //{
            //    c.DefaultWithDetailsFunc = x => x
            //        .Include(y => y.Device).ThenInclude(z => z.Supplier).ThenInclude(o => o.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.Project).ThenInclude(o => o.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.Project).ThenInclude(o => o.Organization).ThenInclude(p => p.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.Project).ThenInclude(o => o.Organization).ThenInclude(p => p.Parent)
            //        .Include(y => y.Device).ThenInclude(z => z.Project).ThenInclude(o => o.Organization).ThenInclude(p => p.Children)
            //        .Include(y => y.Device).ThenInclude(z => z.AreaTenant).ThenInclude(o => o.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.Threshold)
            //        .Include(y => y.Device).ThenInclude(z => z.Extends)
            //        .Include(y => y.Device).ThenInclude(z => z.KeySecrets)
            //        .Include(y => y.Device).ThenInclude(z => z.Mediators)
            //        .Include(y => y.Device).ThenInclude(z => z.DeviceWorkers);
            //});

            //options.Entity<DeviceMediator>(c =>
            //{
            //    c.DefaultWithDetailsFunc = x => x
            //        .Include(y => y.Mediator)
            //        .Include(y => y.Device).ThenInclude(z => z.Supplier)
            //        .Include(y => y.Device).ThenInclude(z => z.Project).ThenInclude(o => o.Organization)
            //        .Include(y => y.Device).ThenInclude(z => z.AreaTenant)
            //        .Include(y => y.Device).ThenInclude(z => z.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.Threshold)
            //        .Include(y => y.Device).ThenInclude(z => z.Sequence)
            //        .Include(y => y.Device).ThenInclude(z => z.Extends)
            //        .Include(y => y.Device).ThenInclude(z => z.KeySecrets)
            //        .Include(y => y.Device).ThenInclude(z => z.Mediators)
            //        .Include(y => y.Device).ThenInclude(z => z.DeviceWorkers);
            //});

            //options.Entity<Mediator>(c =>
            //{
            //    c.DefaultWithDetailsFunc = x => x
            //            .Include(y => y.DeviceMediators)
            //                .ThenInclude(z => z.Mediator)
            //            .Include(y => y.DeviceMediators)
            //                .ThenInclude(z => z.Device)
            //                .ThenInclude(o => o.Project)
            //                .ThenInclude(p => p.Organization)
            //                .ThenInclude(q => q.LocationArea)
            //            .Include(y => y.DeviceMediators)
            //                .ThenInclude(z => z.Device)
            //                .ThenInclude(o => o.Project)
            //                .ThenInclude(p => p.LocationArea)
            //            .Include(y => y.DeviceMediators)
            //                .ThenInclude(z => z.Device)
            //                .ThenInclude(o => o.Project)
            //                .ThenInclude(p => p.Devices)
            //            .Include(y => y.DeviceMediators)
            //                .ThenInclude(z => z.Device)
            //                .ThenInclude(o => o.Project)
            //                .ThenInclude(p => p.ProjectUsers)
            //                .ThenInclude(q => q.User)
            //            .Include(y => y.DeviceMediators)
            //                .ThenInclude(z => z.Device)
            //                .ThenInclude(o => o.Supplier)
            //                .ThenInclude(p => p.Devices)
            //            .Include(y => y.DeviceMediators)
            //                .ThenInclude(z => z.Device)
            //                .ThenInclude(o => o.Supplier)
            //                .ThenInclude(p => p.LocationArea)
            //            .Include(y => y.DeviceMediators)
            //                .ThenInclude(z => z.Device)
            //                .ThenInclude(o => o.LocationArea)
            //            .Include(y => y.DeviceMediators)
            //                .ThenInclude(z => z.Device)
            //                .ThenInclude(o => o.AreaTenant)
            //                .ThenInclude(p => p.TenantDbConnectionStrings)
            //            .Include(y => y.DeviceMediators)
            //                .ThenInclude(z => z.Device)
            //                .ThenInclude(o => o.AreaTenant)
            //                .ThenInclude(p => p.LocationArea)
            //            //.Include(y => y.DeviceMediators)
            //            //.ThenInclude(z => z.Device)
            //            //.ThenInclude(o => o.AreaTenant)
            //            //.ThenInclude(p => p.Devices)//.ThenInclude(p => p.DeviceWorkers)
            //            .Include(y => y.DeviceMediators)
            //                .ThenInclude(z => z.Device)
            //                .ThenInclude(o => o.Threshold)
            //            //.ThenInclude(p => p.Devices)
            //            .Include(y => y.DeviceMediators)
            //                .ThenInclude(z => z.Device)
            //                .ThenInclude(o => o.Sequence)
            //            .Include(y => y.DeviceMediators)
            //                .ThenInclude(z => z.Device)
            //                .ThenInclude(o => o.Extends)
            //            .Include(y => y.DeviceMediators)
            //                .ThenInclude(z => z.Device)
            //                .ThenInclude(o => o.KeySecrets);
            //});

            //options.Entity<DeviceWorker>(c =>
            //{
            //    c.DefaultWithDetailsFunc = x => x
            //        .Include(y => y.AreaTenant).ThenInclude(z => z.LocationArea)
            //        .Include(y => y.Worker)
            //        .Include(y => y.Device).ThenInclude(z => z.Supplier).ThenInclude(o => o.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.Project).ThenInclude(o => o.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.Project).ThenInclude(o => o.Organization).ThenInclude(p => p.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.Project).ThenInclude(o => o.Organization).ThenInclude(p => p.Parent)
            //        .Include(y => y.Device).ThenInclude(z => z.Project).ThenInclude(o => o.Organization).ThenInclude(p => p.Children)
            //        .Include(y => y.Device).ThenInclude(z => z.AreaTenant).ThenInclude(o => o.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.LocationArea)
            //        .Include(y => y.Device).ThenInclude(z => z.Threshold)
            //        .Include(y => y.Device).ThenInclude(z => z.Extends)
            //        .Include(y => y.Device).ThenInclude(z => z.KeySecrets)
            //        .Include(y => y.Device).ThenInclude(z => z.Mediators);
            //});

            //options.Entity<Worker>(c =>
            //{
            //    c.DefaultWithDetailsFunc = x => x
            //        .Include(y => y.DeviceWorkers).ThenInclude(z => z.Device).ThenInclude(o => o.Supplier)
            //        .Include(y => y.DeviceWorkers).ThenInclude(z => z.Device).ThenInclude(o => o.Project)
            //        .Include(y => y.DeviceWorkers).ThenInclude(z => z.Device).ThenInclude(o => o.AreaTenant)
            //        .Include(y => y.DeviceWorkers).ThenInclude(z => z.Device).ThenInclude(o => o.LocationArea)
            //        .Include(y => y.DeviceWorkers).ThenInclude(z => z.Device).ThenInclude(o => o.Threshold)
            //        .Include(y => y.DeviceWorkers).ThenInclude(z => z.Device).ThenInclude(o => o.Sequence)
            //        .Include(y => y.DeviceWorkers).ThenInclude(z => z.Device).ThenInclude(o => o.Extends)
            //        .Include(y => y.DeviceWorkers).ThenInclude(z => z.Device).ThenInclude(o => o.KeySecrets)
            //        .Include(y => y.DeviceWorkers).ThenInclude(z => z.Device).ThenInclude(o => o.Mediators);
            //});

            //options.Entity<AreaTenant>(c =>
            //{
            //    c.DefaultWithDetailsFunc = x => x
            //        .Include(y => y.LocationArea)
            //        .Include(y => y.Devices)
            //        .Include(y => y.DeviceWorkers)
            //        .Include(y => y.TenantDbConnectionStrings).ThenInclude(z => z.AreaTenant);
            //});

            //options.Entity<TenantDbConnectionString>(c =>
            //{
            //    c.DefaultWithDetailsFunc = x => x
            //        .Include(y => y.AreaTenant).ThenInclude(z => z.TenantDbConnectionStrings)
            //        .Include(y => y.AreaTenant).ThenInclude(z => z.Devices)
            //        .Include(y => y.AreaTenant).ThenInclude(z => z.DeviceWorkers);
            //});

            //options.Entity<License>(c =>
            //{
            //    c.DefaultWithDetailsFunc = x => x
            //        .Include(y => y.LicenseResources);
            //});

            //options.Entity<LicenseResource>(c =>
            //{
            //    c.DefaultWithDetailsFunc = x => x
            //        .Include(y => y.License);
            //});

            //options.Entity<Need>(c =>
            //{
            //    c.DefaultWithDetailsFunc = x => x
            //        .Include(y => y.Attachments).ThenInclude(z => z.Need);
            //});

            //options.Entity<NeedAttachment>(c =>
            //{
            //    c.DefaultWithDetailsFunc = x => x
            //        .Include(y => y.Need);
            //});

        });

        //默认配置是不合适，如果需要自己配置，则需要在选择合适SettingProvider，在官方提供了五种方法，
        //它使用的倒序的做的法，先User>Tenant>Global>Configuration>Default

        Configure<AbpSettingOptions>(options =>
        {
            Log.Error($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(AbpSettingOptions)} ....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            //优先级顺序6-1   回退系统从底部 (用户) 到 (自定义) 方向起用用. 6最后执行，所以同配置的6优先
            options.ValueProviders.Add<CustomSettingValueProvider>();//自定义设置值提供程序 6
            options.ValueProviders.Add<DefaultValueSettingValueProvider>();//从设置定义的默认值中获取值 5
            options.ValueProviders.Add<ConfigurationSettingValueProvider>();//从IConfiguration服务中获取值 4 定义了配置文件Settings:节点
            options.ValueProviders.Add<GlobalSettingValueProvider>();//获取设置的全局(系统范围)值. 3
            options.ValueProviders.Add<TenantSettingValueProvider>();//获取当前租户的设置值 2
            options.ValueProviders.Add<UserSettingValueProvider>();//获取当前用户的设置值 1

            ////可以不用写 系统自动发现define
            //options.DefinitionProviders.Add<EmailSettingProvider>();
            //options.DefinitionProviders.Add<NetCoreSettingDefinitionProvider>();
        });
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainModule)} End ConfigureServices ....");


    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainModule)} Start PostConfigureServices ....");
        base.PostConfigureServices(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainModule)} End PostConfigureServices ....");
    }

    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainModule)} Start OnPreApplicationInitialization ....");
        base.OnPreApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainModule)} End OnPreApplicationInitialization ....");
    }
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainModule)} Start OnApplicationInitialization ....");
        base.OnApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainModule)} End OnApplicationInitialization ....");
    }

    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainModule)} Start OnPostApplicationInitialization ....");
        base.OnPostApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainModule)} End OnPostApplicationInitialization ....");
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainModule)} Start OnApplicationShutdown ....");
        base.OnApplicationShutdown(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetDomainModule)} End OnApplicationShutdown ....");
    }

}
