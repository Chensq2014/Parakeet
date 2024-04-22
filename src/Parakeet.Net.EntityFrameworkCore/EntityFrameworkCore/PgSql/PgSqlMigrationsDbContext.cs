using Common;
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
            builder.Configure(true);


            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(PgSqlMigrationsDbContext)} OnModelCreating end.............");
        }
    }
}