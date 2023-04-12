using Microsoft.EntityFrameworkCore;
using Parakeet.Net.Cache;
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
    [ConnectionStringName(CustomerConsts.MySqlConnectionStringName)]
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
            
            base.OnModelCreating(builder);
            builder.Configure(true);
            
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(MySqlMigrationsDbContext)} OnModelCreating end.............");
        }
    }
}