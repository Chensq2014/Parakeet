using Microsoft.EntityFrameworkCore;
using Parakeet.Net.Cache;
using Parakeet.Net.Entities;
using Parakeet.Net.Entities.Devices;
using Parakeet.Net.Entities.Houses;
using Parakeet.Net.Entities.LocationAreas;
using Parakeet.Net.Entities.Mediators;
using Parakeet.Net.Entities.Needs;
using Parakeet.Net.Entities.Organizations;
using Parakeet.Net.Entities.Products;
using Parakeet.Net.Entities.Projects;
using Parakeet.Net.Entities.SecurePolicies;
using Parakeet.Net.Entities.Suppliers;
using Parakeet.Net.Entities.System;
using Parakeet.Net.Entities.Thresholds;
using Parakeet.Net.Entities.Workers;
using Parakeet.Net.Users;
using Serilog;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Parakeet.Net.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName(CustomerConsts.ConnectionStringName)]
public class NetDbContext :
    AbpDbContext<NetDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */
    public static string TablePrefix { get; set; } = CustomerConsts.DefaultDbTablePrefix;

    public static string Schema { get; set; } = CustomerConsts.DefaultDbSchema;

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }



    #region 实体集(EF默认NetCore属性名称对应表名,可在OnModelCreating中配置表名)


    //public DbSet<AppUser> AbpUsers { get; set; }

    /// <summary>
    /// 位置区域（全国省市区城乡等区域代码位置信息）
    /// </summary>
    public DbSet<LocationArea> LocationAreas { get; set; }

    /// <summary>
    /// 消息表
    /// </summary>
    public DbSet<Notify> Notifies { get; set; }

    /// <summary>
    /// 组织机构表
    /// </summary>
    public DbSet<Organization> Organizations { get; set; }

    /// <summary>
    /// 机构岗位用户表
    /// </summary>
    public DbSet<OrganizationUser> OrganizationUsers { get; set; }

    /// <summary>
    /// 项目表
    /// </summary>
    public DbSet<Project> Projects { get; set; }

    /// <summary>
    /// 项目用户表
    /// </summary>
    public DbSet<ProjectUser> ProjectUsers { get; set; }

    /// <summary>
    /// 项目附件表
    /// </summary>
    public DbSet<ProjectAttachment> ProjectAttachments { get; set; }

    /// <summary>
    /// 需求表
    /// </summary>
    public DbSet<Need> Needs { get; set; }

    /// <summary>
    /// 需求附件表
    /// </summary>
    public DbSet<NeedAttachment> NeedAttachments { get; set; }

    /// <summary>
    /// 区域租户表
    /// </summary>
    public DbSet<AreaTenant> AreaTenants { get; set; }

    /// <summary>
    /// 区域租户字符串连接表
    /// </summary>
    public DbSet<TenantDbConnectionString> TenantDbConnectionStrings { get; set; }

    /// <summary>
    /// 供应商表
    /// </summary>
    public DbSet<Supplier> Suppliers { get; set; }

    /// <summary>
    /// 阈值管理
    /// </summary>
    public DbSet<Threshold> Thresholds { get; set; }

    /// <summary>
    /// 设备表
    /// </summary>
    public DbSet<Device> Devices { get; set; }

    /// <summary>
    /// 设备扩展表
    /// </summary>
    public DbSet<DeviceExtend> DeviceExtends { get; set; }

    /// <summary>
    /// 设备区域转发密钥表
    /// </summary>
    public DbSet<DeviceKeySecret> DeviceKeySecrets { get; set; }

    /// <summary>
    /// 设备序号表
    /// </summary>
    public DbSet<DeviceSequence> DeviceSequences { get; set; }

    /// <summary>
    /// 设备转发表
    /// </summary>
    public DbSet<DeviceMediator> DeviceMediators { get; set; }

    /// <summary>
    /// 设备人员表
    /// </summary>
    public DbSet<DeviceWorker> DeviceWorkers { get; set; }

    /// <summary>
    /// 设备数据模拟表
    /// </summary>
    public DbSet<DeviceAnalogRule> DeviceAnalogRules { get; set; }

    /// <summary>
    /// 劳务人员表
    /// </summary>
    public DbSet<Worker> Workers { get; set; }

    /// <summary>
    /// 中介器表
    /// </summary>
    public DbSet<Mediator> Mediators { get; set; }

    ///// <summary>
    ///// Tcp包头管理 不用存表放在配置文件或者redis缓存更合适
    ///// </summary>
    //public DbSet<PacketHandler> PacketHandlers { get; set; }

    /// <summary>
    /// 许可证
    /// </summary>
    public DbSet<License> Licenses { get; set; }

    /// <summary>
    /// 许可证资源
    /// </summary>
    public DbSet<LicenseResource> LicenseResources { get; set; }

    ///// <summary>
    ///// 小区
    ///// </summary>
    //public DbSet<Section> Sections { get; set; }

    /// <summary>
    /// 房间
    /// </summary>
    public DbSet<House> Houses { get; set; }

    /// <summary>
    /// 产品
    /// </summary>
    public DbSet<Product> Products { get; set; }

    /// <summary>
    /// 安全策略
    /// </summary>
    public DbSet<SecurePolicy> SecurePolicies { get; set; }
    #endregion


    #endregion

    public NetDbContext(DbContextOptions<NetDbContext> options)
        : base(options)
    {

    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Log.Logger.Information($"DbContextOptionsBuilder EnableSensitiveDataLogging 启用/禁用sql日志");
        optionsBuilder.EnableSensitiveDataLogging(false);//启用/禁用sql日志
        //optionsBuilder.UseLoggerFactory(this._iLoggerFactory);
        //optionsBuilder.UseLoggerFactory(new CustomEFLoggerFactory());

        //microsoft.entityframeworkcore.proxies\3.1.8\lib\netstandard2.0\Microsoft.EntityFrameworkCore.Proxies.dll
        //ProxiesExtensions 延迟加载代理通过将ILazyLoader服务注入实体来工作，如实体类型构造函数中所述。
        //例如：此方法不要求从中继承实体类型，也不要求导航属性是虚拟的，
        //并且允许在附加到上下文后使用new to lazy load创建实体实例。
        //optionsBuilder.UseLazyLoadingProxies();

        //这里可以直接设置数据库连接字符串
        //optionsBuilder.UseSqlServer("Server=.;Database=advanced11;User id=sa;password=Passw0rd");
        //optionsBuilder.UseSqlServer(this._IConfiguration.GetConnectionString("JDDbConnection"));

        //var currentYear = DateTime.Now.Year;//按年分库  某一年数据库连接字符串
        //var connectString = $"Master_{AppConstants.DB_AREA}_{currentYear}";
        //optionsBuilder.UseNpgsql(connectString);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        if (builder.IsUsingPostgreSql())
        {
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、使用的PostGreSql");
        }
        if (builder.IsUsingMySQL())
        {
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、使用的MySQL");
        }
        if (builder.IsUsingSqlServer())
        {
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、使用的SqlServer");
        }
        Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(NetDbContext)} OnModelCreating start.............");

        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(NetConsts.DbTablePrefix + "YourEntities", NetConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
        
        //builder.ConfigureNetCore();//以前的扩展 现在不需要了
        Log.Logger.Information($"This is {nameof(NetDbContext)} OnModelCreating end.............");
    }
}
