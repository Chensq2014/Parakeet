using Microsoft.EntityFrameworkCore;
using Parakeet.Net.Cache;
using Serilog;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Parakeet.Net.EntityFrameworkCore
{
    /* This DbContext is only used for database migrations.
     * It is not used on runtime. See NetCoreDbContext for the runtime DbContext.
     * It is a unified model that includes configuration for
     * all used modules and your application.
     */
    [ConnectionStringName(CustomerConsts.MutiTenantConnectionStringName)]
    public class PgSqlMutiTenantMigrationsDbContext : AbpDbContext<PgSqlMutiTenantMigrationsDbContext>
    {
        // Tenant Management
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }
        
        public PgSqlMutiTenantMigrationsDbContext(DbContextOptions<PgSqlMutiTenantMigrationsDbContext> options) : base(options)
        {
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(PgSqlMutiTenantMigrationsDbContext)} DbContextOptions.............");
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder.IsUsingPostgreSql())
            {
                Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、MutiTenant使用的PostGreSql");
            }
            if (builder.IsUsingMySQL())
            {
                Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、MutiTenant使用的MySQL");
            }
            if (builder.IsUsingSqlServer())
            {
                Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、MutiTenant使用的SqlServer");
            }
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(PgSqlMutiTenantMigrationsDbContext)} OnModelCreating start.............");

            //配置多租户表
            builder.ConfigureTenantManagement();
            base.OnModelCreating(builder);

            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(PgSqlMutiTenantMigrationsDbContext)} OnModelCreating end.............");
        }
    }
}