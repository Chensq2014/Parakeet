using Common.Interfaces;
using Common.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Parakeet.Net.Repositories;
using Serilog;
using System;
using System.Threading;
using Volo.Abp;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Parakeet.Net.EntityFrameworkCore;

[DependsOn(
    typeof(NetDomainModule),
    typeof(NetMultiTenancyModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpOpenIddictEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqlServerModule),
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
    typeof(AbpTenantManagementEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule)
    )]
public class NetEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetEntityFrameworkCoreModule)} Start PreConfigureServices ....");

        //NodaTime：tstzrange 和 daterange 默认映射到 Interval 和 DateInterval
        // 使用 NodaTime 时，PostgreSQL 列作为 Interval 属性而不是 （#4070） 进行基架，而列作为 （#1998） 的 DateInterval 属性进行基架。
        // tstzrangeNpgsqlRange<Instant>daterangeNpgsqlRange<LocalDateTime>

        //默认情况下，日期/时间最小/最大值现在转换为 PostgreSQL 无穷大值
        //PostgreSQL 具有时间戳和日期的特殊值和值，这些值晚于其他值和早于其他值。Npgsql 支持映射，
        //并通过连接字符串参数映射到这些无穷大值， 默认情况下禁用该参数。此行为现在默认处于启用状态，
        //因为很少用作实际时间戳/日期，并且该参数已被删除。若要禁用无穷大转换，请在应用程序开始时添加以下内容：
        //infinity-infinityDateTime.MaxValueMinValueConvert Infinity DateTimeDateTime.MaxValueMinValueConvert Infinity DateTime
        //AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        // https://www.npgsql.org/efcore/release-notes/6.0.html#opting-out-of-the-new-timestamp-mapping-logic
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        NetEfCoreEntityExtensionMappings.Configure();

        #region IServiceCollection.OnRegistred 事件
        ////(注册到依赖注入的每个服务上执行一个操作)
        //context.Services.OnRegistred(ctx =>
        //{
        //    Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_注册{ctx.ImplementationType.Name}...");
        //    //该回调(callback)通常用于向服务添加拦截器
        //    if (ctx.ImplementationType.IsDefined(typeof(MyLogAttribute), true))
        //    {
        //        ctx.Interceptors.TryAdd<MyLogInterceptor>();
        //    }
        //});
        #endregion

        #region 查看源码发现abp默认没有依赖注入泛型接口，需要手动注册  
        //建议使用在Dbcontext上添加AutoRepositoryTypes 属性 
        //但测试AutoRepositoryTypes属性添加后运行错误,仍然需要在模块中显示注册

        //IocManager.IocContainer.Register(Component.For(typeof(INetCoreRepository<>))
        //    .ImplementedBy(typeof(NetRepositoryBase<>)).LifestyleTransient());
        //IocManager.IocContainer.Register(Component.For(typeof(INetCoreRepository<,>))
        //    .ImplementedBy(typeof(NetRepositoryBase<,>)).LifestyleTransient());
        ////IocManager.IocContainer.Register(
        ////    Classes.FromAssembly(typeof(NetEntityFrameworkModule).GetAssembly())//这种方式注入有bug
        ////        //.IncludeNonPublicTypes()
        ////        .BasedOn<IRepository>()
        ////        //.If(type => type.GetTypeInfo().Namespace == typeof(INetCoreRepository<>).GetTypeInfo().Namespace
        ////        //            || type.GetTypeInfo().Namespace == typeof(INetCoreRepository<,>).GetTypeInfo().Namespace)
        ////        .WithService.Self()
        ////        .WithService.DefaultInterfaces()
        ////        .LifestyleTransient()
        ////);

        //查看源码，仓储的注册使用的是如下方法，但必须指定类型才可以
        //context.Services.AddDefaultRepository(typeof(INetCoreRepository<>), 
        //    EntityHelper.FindPrimaryKeyType(typeof(NetRepositoryBase<>)));
        //context.Services.AddDefaultRepository(typeof(INetCoreRepository<,>), 
        //    EntityHelper.FindPrimaryKeyType(typeof(NetRepositoryBase<,>)));

        //vnext版本直接这样在容器里直接注册 
        context.Services.TryAddTransient(typeof(INetRepository<>), typeof(NetRepositoryBase<>));
        context.Services.TryAddTransient(typeof(INetRepository<,>), typeof(NetRepositoryBase<,>));

        //// 添加自定义指定类型仓储 为什么dbcontext不直接scope注册呢？同一次请求并发？
        //context.Services.AddAbpDbContext<NetDbContext>(options =>
        //{
        //    options.AddDefaultRepositories();
        //    //options.AddRepository<AppUser, EfCoreIdentityUserRepository>();
        //});
        #endregion
        ////禁用依赖注入自动注册服务
        //SkipAutoServiceRegistration = true;
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetEntityFrameworkCoreModule)} Start ConfigureServices ....");

        #region 注册DbContext  配置AbpDbContextOptions

        //告诉容器 构造NetDbContext 时把options当作参数传递给构造函数
        
        context.Services.AddAbpDbContext<NetDbContext>(options =>
        {
            Log.Error($"{{0}}",
                $"{CacheKeys.LogCount++}、AddAbpDbContex配置{nameof(IAbpDbContextRegistrationOptionsBuilder)}告诉容器 构造{nameof(NetDbContext)} 时把options当作参数传递给构造函数....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            /* Remove "includeAllEntities: true" to create
             * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
        });
        context.Services.AddAbpDbContext<SqlServerMigrationsDbContext>(options =>
        {
            Log.Error($"{{0}}",
                $"{CacheKeys.LogCount++}、AddAbpDbContex配置{nameof(IAbpDbContextRegistrationOptionsBuilder)}告诉容器 构造{nameof(SqlServerMigrationsDbContext)} 时把options当作参数传递给构造函数....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            /* Remove "includeAllEntities: true" to create
             * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
        });
        context.Services.AddAbpDbContext<MySqlMigrationsDbContext>(options =>
        {
            Log.Error($"{{0}}",
                $"{CacheKeys.LogCount++}、AddAbpDbContex配置{nameof(IAbpDbContextRegistrationOptionsBuilder)}告诉容器 构造{nameof(MySqlMigrationsDbContext)} 时把options当作参数传递给构造函数....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            /* Remove "includeAllEntities: true" to create
             * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
        });
        context.Services.AddAbpDbContext<PgSqlMigrationsDbContext>(options =>
        {
            Log.Error($"{{0}}",
                $"{CacheKeys.LogCount++}、AddAbpDbContex配置{nameof(IAbpDbContextRegistrationOptionsBuilder)}告诉容器 构造{nameof(PgSqlMigrationsDbContext)} 时把options当作参数传递给构造函数....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            /* Remove "includeAllEntities: true" to create
             * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        Configure<AbpDbContextOptions>(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(AbpDbContextOptions)} UseSqlServer or UseNpgsql or UseMySQL....  ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");

            /* The main point to change your DBMS.
             * See also NetMigrationsDbContextFactory for EF Core tooling. */
            options.UseNpgsql();
            //options.UseMySQL(sqlOption =>
            //{
            //    sqlOption.EnableRetryOnFailure();
            //});
            //options.UseSqlServer();
        });
        //context.Services.AddDefaultRepository(
        //    typeof(AuditLogAction),
        //    typeof(EfCoreRepository<AbpAuditLoggingDbContext, AuditLogAction, Guid>)
        //);
        //context.Services.AddDefaultRepository(
        //    typeof(AuditLog),
        //    typeof(EfCoreRepository<AbpAuditLoggingDbContext, AuditLog, Guid>)
        //);
        //context.Services.AddDefaultRepository(
        //    typeof(BackgroundJobRecord),
        //    typeof(EfCoreRepository<BackgroundJobsDbContext, BackgroundJobRecord, Guid>)
        //);

        #endregion

        #region 一旦禁用依赖注入自动注册服务,你应该手动注册你的服务.
        ////按Module程序集批量注册
        //context.Services.AddAssemblyOf<NetCoreEntityFrameworkCoreModule>();

        ////单独注册一个transient实例
        //context.Services.AddTransient<>()
        ////单独注册一个singleton实例
        //context.Services.AddSingleton<TaxCalculator>(new TaxCalculator(taxRatio: 0.18));
        ////单独注册一个从IServiceProvider解析得来的工厂方法
        //context.Services.AddScoped<ITaxCalculator>(sp => sp.GetRequiredService<TaxCalculator>());
        #endregion

        #region 替换默认Service 三种方式任选一种  ReplaceService 

        ////1、在resolver实现类(ConnectionStringResolver)上添加 替换默认服务的属性 [Dependency(ReplaceServices = true)]
        ////2、使用context.Services.Replace方法
        ////context.Services.Replace(ServiceDescriptor.Transient(typeof(IConnectionStringResolver), typeof(ConnectionStringResolver)));
        ////context.Services.Replace(new ServiceDescriptor(typeof(IConnectionStringResolver), typeof(ConnectionStringResolver), ServiceLifetime.Transient));
        //3、使用DbContextOptionsBuilder的配置方法 内部调用ReplaceService方法
        //Configure<DbContextOptionsBuilder>(optionsBuilder =>
        //{
        //    optionsBuilder.ReplaceService<IConnectionStringResolver, ConnectionStringResolver>();
        //});

        #endregion

        #region 缓存放入domain层注册

        //Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(NetEntityFrameworkCoreModule)} 注册IFreeSql、ICacheContainer、DevicePool....");

        ////注册freeSql、ICacheContainer、 DevicePool高速缓存单例
        //var dbConnectionString = context.Services.GetConfiguration()
        //    .GetConnectionString(CustomerConsts.ConnectionStringName);
        //var freeSql = new FreeSqlBuilder().UseConnectionString(DataType.PostgreSQL, dbConnectionString).Build();
        //ConfigFluentApi(freeSql.CodeFirst);
        //context.Services.TryAddSingleton(freeSql);
        //context.Services.TryAddSingleton<DevicePool>();
        //context.Services.TryAddSingleton<LicensePool>();
        //context.Services.AddCachePool();//domain模块已调用

        #endregion

        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetEntityFrameworkCoreModule)} End ConfigureServices ....");

    }


    ///// <summary>
    ///// IFreeSql codeFirst的流式api
    ///// </summary>
    ///// <param name="codeFirst">代码优先接口(IFreeSql)</param>
    //private static void ConfigFluentApi(ICodeFirst codeFirst)
    //{
    //    codeFirst.ConfigEntity<Device>(x =>
    //    {
    //        x.Name(GetTableName<Device>());
    //        x.Navigate(y => y.Supplier, "SupplierId");
    //        //x.Navigate(y => y.AreaTenant, "AreaTenantId");
    //        x.Navigate(y => y.Extends, "DeviceId");
    //    });

    //    codeFirst.ConfigEntity<DeviceExtend>(x =>
    //    {
    //        x.Name(GetTableName<DeviceExtend>());
    //        x.Navigate(y => y.Device, "DeviceId");
    //    });

    //    codeFirst.ConfigEntity<DeviceMediator>(x =>
    //    {
    //        x.Name(GetTableName<DeviceMediator>());
    //        x.Navigate(y => y.Device, "DeviceId");
    //        x.Navigate(y => y.Mediator, "MediatorId");
    //    });

    //    codeFirst.ConfigEntity<Supplier>(x =>
    //    {
    //        x.Name(GetTableName<Supplier>());
    //    });

    //    codeFirst.ConfigEntity<Mediator>(x => { x.Name(GetTableName<Mediator>()); });

    //    codeFirst.ConfigEntity<TenantDbConnectionString>(x =>
    //    {
    //        x.Navigate(y => y.AreaTenant, "AreaTenantId");
    //        x.Name(GetTableName<TenantDbConnectionString>());
    //    });

    //    codeFirst.ConfigEntity<AreaTenant>(x =>
    //    {
    //        x.Name(GetTableName<AreaTenant>());
    //    });
    //    codeFirst.ConfigEntity<License>(x =>
    //    {
    //        x.Name(GetTableName<License>());
    //    });
    //    codeFirst.ConfigEntity<LicenseResource>(x =>
    //    {
    //        x.Name(GetTableName<LicenseResource>());
    //    });
    //    //codeFirst.ConfigEntity<PacketHandler>(x =>
    //    //{
    //    //    x.Name(GetTableName<PacketHandler>());
    //    //});

    //    //codeFirst.ConfigEntity<KeySecret>(x =>
    //    //{
    //    //    x.Name(GetTableName<KeySecret>());
    //    //});

    //    codeFirst.ConfigEntity<Worker>(x =>
    //    {
    //        x.Name(GetTableName<Worker>());
    //        x.Property(t => t.IdPhoto).DbType("text");
    //        x.Property(t => t.Photo).DbType("text");
    //        x.Property(t => t.InfraredPhoto).DbType("text");
    //    });
    //    codeFirst.ConfigEntity<DeviceWorker>(x =>
    //    {
    //        x.Name(GetTableName<DeviceWorker>());
    //        x.Navigate(t => t.AreaTenant, "AreaTenantId");
    //        x.Navigate(y => y.Worker, "WorkerId");
    //    });
    //}



    ///// <summary>
    ///// 获取表名
    ///// </summary>
    ///// <typeparam name="TEntity">实体类</typeparam>
    ///// <returns></returns>
    //private static string GetTableName<TEntity>() where TEntity : class
    //{
    //    var type = typeof(TEntity);
    //    var tableName = type.GetCustomAttribute<TableAttribute>()?.Name ?? $"{type.Name}s";
    //    return tableName;
    //}

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetEntityFrameworkCoreModule)} Start PostConfigureServices ....");
        base.PostConfigureServices(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetEntityFrameworkCoreModule)} End PostConfigureServices ....");
    }


    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetEntityFrameworkCoreModule)} Start OnPreApplicationInitialization ....");
        base.OnPreApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetEntityFrameworkCoreModule)} End OnPreApplicationInitialization ....");
    }
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetEntityFrameworkCoreModule)} Start OnApplicationInitialization ....");
        base.OnApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetEntityFrameworkCoreModule)} End OnApplicationInitialization ....");
    }

    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetEntityFrameworkCoreModule)} Start OnPostApplicationInitialization ....");
        base.OnPostApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetEntityFrameworkCoreModule)} End OnPostApplicationInitialization ....");
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetEntityFrameworkCoreModule)} Start OnApplicationShutdown ....");
        base.OnApplicationShutdown(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetEntityFrameworkCoreModule)} End OnApplicationShutdown ....");
    }
}
