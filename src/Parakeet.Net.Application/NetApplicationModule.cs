using Common;
using Common.Storage;
using Common.Test;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Parakeet.Net.ServiceGroup;
using Serilog;
using System.Threading;
using Common.Users;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace Parakeet.Net;

[DependsOn(
    typeof(CommonModule),
    typeof(NetDomainModule),
    typeof(NetMultiTenancyModule),
    typeof(AbpAccountApplicationModule),
    typeof(NetApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class NetApplicationModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetApplicationModule)} Start PreConfigureServices ....3");
        ////// 将用户自定义拦截器加入待依赖注入注册拦截器集合中，最终由autofac自动调用注册方法
        //context.Services.OnRegistred(CacheLockInterceptorRegistrar.Register);//autofac dll文件引用bug 待解决
        ////context.Services.AddTransient(typeof(CastleAbpInterceptorAdapter<CacheLockInterceptor>));//是否可用
        ////context.Services.AddTransient(typeof(CacheLockInterceptor));//是否需要添加到容器,框架默认应该有处理?

        ////不需要注册 IBaseNetAppService
        //context.Services.TryAddTransient(typeof(IBaseNetAppService<>), typeof(BaseNetAppService<>));
        //context.Services.TryAddTransient(typeof(IBaseNetAppService<,>), typeof(BaseNetAppService<,>));

        //IEmailSender默认已经注入 配置文件中需要用到AbpMailKitOptions 这个节点
        //context.Services.TryAddTransient(typeof(IEmailSender),typeof(MailKitSmtpEmailSender));

        base.PreConfigureServices(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetApplicationModule)} End PreConfigureServices ....");
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetApplicationModule)} Start ConfigureServices ....");

        Configure<AbpAutoMapperOptions>(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(AbpAutoMapperOptions)}_{nameof(NetApplicationModule)} 模块继承自Profile类的批量添加AutoMapper....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            //继承自Profile类的批量AutoMapper 直接添加这个ApplicationModule即可
            options.AddMaps<NetApplicationModule>(true);
            //options.AddProfile<NetApplicationAutoMapperProfile>(true); //单独添加某一个profile
        });

        Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、TryAddSingleton与AddHostedService方式注册后台定时任务 ....");
        //context.Services.TryAddSingleton<ClearTempDataBackgroundWorker>();//单例注入 这里可以不写，因为abp框架自动注入了

        #region 后台定时任务 HostServices
        ////context.Services.AddHostedService<CustomBackgroundJob>();//单例注入
        //context.Services.AddHostedService<DeviceAnalogRuleBackgroundTaskService>();//暂不开启
        #endregion
        //var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
        //workManager.Add(IocManager.Resolve<RefreshSpiderTokenWorker>());//Spider服务，确保该服务在最前面，因为需要提前获取 Token
        //workManager.Add(IocManager.Resolve<CleanLockCacheWorker>());//密码错误锁定自动任务
        //workManager.Start();

        Configure<AbpBackgroundJobOptions>(options =>
        {
            ////禁用作业执行
            //options.IsJobExecutionEnabled = true;
        });

        //配置appJson节点反射类+微服务及节点及微服务节点的过滤器
        context.Services.RegisterHttpsApi();

        ////用户自定义接口锁及默认实现及注册
        //context.Services.AddLock();

        #region HttpFactory配置

        context.Services.AddHttpClient(nameof(NetApplicationModule));
        //context.Services.AddHttpClient(nameof(NetApplicationModule), c =>
        //{
        //    c.DefaultRequestHeaders.Add("Accept", "application/json");
        //})
        //.AddPolicyHandler(request => Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(config.GetValue<int>("ReceiveServiceSource:TimeOut"))))
        //.AddTransientHttpErrorPolicy(builder => builder.RetryAsync(config.GetValue<int>("ReceiveServiceSource:RetryCount")));

        #endregion HttpFactory配置

        #region 同个接口多个实现实例

        //context.Services.TryAddScoped<IReverseCommand, AddPersonCommand>();
        //context.Services.TryAddScoped<IReverseCommand, DeletePersonCommand>();

        ////调用处 _serviceProvider.Resolve<IReverseCommand>(typeof(AddPersonCommand).FullName);//HandlerType 实现类里唯一可自定义
        ////  var command = _serviceProvider.Resolve<IReverseCommand>($"{area}_{device.Supplier.Code}_{request.Command}");
        ////  await command.Execute(device, request.Body);
        #endregion

        #region 测试

        context.Services.AddTransient<IOperationTransient, Test.Operation>();
        context.Services.AddScoped<IOperationScoped, Test.Operation>();
        context.Services.AddSingleton<IOperationSingleton, Test.Operation>();
        context.Services.AddTransient<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
        #endregion

        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetApplicationModule)} End ConfigureServices ....");
    }
    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetApplicationModule)} Start PostConfigureServices ....");

        base.PostConfigureServices(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetApplicationModule)} End PostConfigureServices ....");
    }

    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetApplicationModule)} Start OnPreApplicationInitialization ....");
        base.OnPreApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetApplicationModule)} End OnPreApplicationInitialization ....");
    }
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetApplicationModule)} Start OnApplicationInitialization ....");

        #region 初始化GrpcOption

        //var optionsMonitor = context.ServiceProvider.GetRequiredService<IOptionsMonitor<ROClientOptionDto>>();
        //var grpcOption = optionsMonitor.CurrentValue;
        //GrpcOption.Instance.Initialize(grpcOption.AppId, grpcOption.AppKey, grpcOption.AppSecret, grpcOption.ServerUrl);
        #endregion

        #region abp的自定义定时任务

        ////第一种方式：
        ////abp继承自PeriodicBackgroundWorkerBase的定时任务 需要这样调用
        //var workManager = context.ServiceProvider.GetService<IBackgroundWorkerManager>();
        //workManager.Add(context.ServiceProvider.GetService<ClearTempDataBackgroundWorker>());
        ////workManager.Add(context.ServiceProvider.GetService<XiamenHuizhanBackGroundWorker>());
        ////workManager.Add(context.ServiceProvider.GetService<XiamenHuizhanSectionTwoGateHistroryBackGroundWroker>());
        ////workManager.StartAsync();//这句都可以不用写

        //第二种方式：
        ////或者这样写：abp默认把当前ClearTempDataBackgroundWorker 添加到 IBackgroundWorkerManager 管理器中 与上面的代码是等效的
        ////context.ServiceProvider.GetRequiredService<IBackgroundWorkerManager>().Add(context.ServiceProvider.GetService<ClearTempDataBackgroundWorker>());

        //第三种方式：
        //context.AddBackgroundWorker<ClearTempDataBackgroundWorker>();
        #endregion

        base.OnApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetApplicationModule)} End OnApplicationInitialization ....");
    }

    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetApplicationModule)} Start OnPostApplicationInitialization ....");
        base.OnPostApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetApplicationModule)} End OnPostApplicationInitialization ....");
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetApplicationModule)} Start OnApplicationShutdown ....");
        base.OnApplicationShutdown(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetApplicationModule)} End OnApplicationShutdown ....");
    }
}
