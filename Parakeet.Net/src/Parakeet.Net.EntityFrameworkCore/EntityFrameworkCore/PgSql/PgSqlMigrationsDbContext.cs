using Microsoft.EntityFrameworkCore;
using Parakeet.Net.Cache;
using Parakeet.Net.Storage;
using Serilog;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Parakeet.Net.EntityFrameworkCore
{
    /* This DbContext is only used for database migrations.
     * It is not used on runtime. See NetCoreDbContext for the runtime DbContext.
     * It is a unified model that includes configuration for
     * all used modules and your application.
     */
    [ConnectionStringName(CustomerConsts.PgSqlConnectionStringName)]
    public class PgSqlMigrationsDbContext : AbpDbContext<PgSqlMigrationsDbContext>
    {
        public PgSqlMigrationsDbContext(DbContextOptions<PgSqlMigrationsDbContext> options) : base(options)
        {
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(PgSqlMigrationsDbContext)} DbContextOptions.............");
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
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(PgSqlMigrationsDbContext)} OnModelCreating start.............");
            base.OnModelCreating(builder);
            /* Include modules to your migration db context */

            builder.ConfigurePermissionManagement();
            builder.ConfigureSettingManagement();
            builder.ConfigureBackgroundJobs();
            builder.ConfigureAuditLogging();
            builder.ConfigureIdentity();
            //builder.ConfigureIdentityServer();
            builder.ConfigureFeatureManagement();
            builder.ConfigureTenantManagement();

            /* Configure customizations for entities from the modules included  */

            //builder.Entity<IdentityUser>(b =>
            //{
            //    b.ConfigureCustomUserProperties();
            //});

            /* Configure your own tables/entities inside the ConfigureNetCore method */

            //builder.ConfigureNetCore(true);
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(PgSqlMigrationsDbContext)} OnModelCreating end.............");
        }
    }
}