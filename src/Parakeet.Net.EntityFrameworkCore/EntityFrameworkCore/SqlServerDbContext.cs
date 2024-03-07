using Microsoft.EntityFrameworkCore;
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
[ConnectionStringName("Default")]
public class SqlServerDbContext :
    AbpDbContext<SqlServerDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

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
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options)
        : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //Log.Logger.Information($"DbContextOptionsBuilder EnableSensitiveDataLogging 启用/禁用sql日志");
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
        //if (builder.IsUsingPostgreSql())
        //{
        //    Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、使用的PostGreSql");
        //}
        //if (builder.IsUsingMySQL())
        //{
        //    Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、使用的MySQL");
        //}
        //if (builder.IsUsingSqlServer())
        //{
        //    Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、使用的SqlServer");
        //}
        //Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(NetCoreDbContext)} OnModelCreating start.............");



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
    }
}
