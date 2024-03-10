﻿using Common;
using Common.Storage;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Parakeet.Net.EntityFrameworkCore
{
    /* This DbContext is only used for database migrations.
     * It is not used on runtime. See NetCoreDbContext for the runtime DbContext.
     * It is a unified model that includes configuration for
     * all used modules and your application.
     */
    [ConnectionStringName(CommonConsts.SqlServerConnectionStringName)]
    public class SqlServerMigrationsDbContext : AbpDbContext<SqlServerMigrationsDbContext>
    {
        public SqlServerMigrationsDbContext(DbContextOptions<SqlServerMigrationsDbContext> options) : base(options)
        {
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(SqlServerMigrationsDbContext)} DbContextOptions.............");
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
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(SqlServerMigrationsDbContext)} OnModelCreating start.............");

            builder.Configure(true);
            base.OnModelCreating(builder);

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

            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(SqlServerMigrationsDbContext)} OnModelCreating end.............");
        }
    }
}