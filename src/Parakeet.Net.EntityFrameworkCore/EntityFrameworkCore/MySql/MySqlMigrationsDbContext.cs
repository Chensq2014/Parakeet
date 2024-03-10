using Common;
using Common.Storage;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
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
    [ConnectionStringName(CommonConsts.MySqlConnectionStringName)]
    public class MySqlMigrationsDbContext : AbpDbContext<MySqlMigrationsDbContext>
    {
        public MySqlMigrationsDbContext(DbContextOptions<MySqlMigrationsDbContext> options) : base(options)
        {
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(MySqlMigrationsDbContext)} DbContextOptions.............");
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
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(MySqlMigrationsDbContext)} OnModelCreating start.............");


            #region Include modules to your migration db context
            /* Include modules to your migration db context */

            //builder.ConfigurePermissionManagement();
            //builder.ConfigureSettingManagement();
            //builder.ConfigureBackgroundJobs();
            //builder.ConfigureAuditLogging();
            //builder.ConfigureIdentity();
            //builder.ConfigureOpenIddict();
            //builder.ConfigureFeatureManagement();
            //builder.ConfigureTenantManagement();

            #endregion

            builder.Configure(true);
            base.OnModelCreating(builder);
            
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(MySqlMigrationsDbContext)} OnModelCreating end.............");
        }
    }
}