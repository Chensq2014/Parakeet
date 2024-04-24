using Common;
using Common.Entities;
using Common.Storage;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Parakeet.Net.EntityFrameworkCore
{
    /* This DbContext is only used for database migrations.
     * It is not used on runtime. See ParakeetDbContext for the runtime DbContext.
     * It is a unified model that includes configuration for
     * all used modules and your application.
     */
    [ConnectionStringName(CommonConsts.PgSqlConnectionStringName)]
    public class PgSqlMigrationsDbContext : AbpDbContext<PgSqlMigrationsDbContext>
    {
        public PgSqlMigrationsDbContext(DbContextOptions<PgSqlMigrationsDbContext> options) : base(options)
        {
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(PgSqlMigrationsDbContext)} DbContextOptions.............");
        }

        #region 租户公共数据结构
        //public DbSet<EnvironmentRecord> EnvironmentRecords { get; set; }
        //public DbSet<CraneAlarm> CraneAlarms { get; set; }
        //public DbSet<CraneRecord> CraneRecords { get; set; }
        //public DbSet<CraneBasic> CraneBasics { get; set; }

        #endregion

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

            builder.Configure(true);
            base.OnModelCreating(builder);

            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(PgSqlMigrationsDbContext)} OnModelCreating end.............");
        }
    }
}