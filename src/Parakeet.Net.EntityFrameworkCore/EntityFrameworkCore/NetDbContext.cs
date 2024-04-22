using Common;
using Common.Entities;
using Common.Storage;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Parakeet.Net.EntityFrameworkCore
{
    /// <summary>
    /// 租户运行时数据库
    ///  /* This is your actual DbContext used on runtime.
    ///  * It includes only your entities.
    ///  * It does not include entities of the used modules, because each module has already
    ///  * its own DbContext class. If you want to share some database tables with the used modules,
    ///  * just create a structure like done for AppUser.
    ///  *
    ///  * Don't use this DbContext for database migrations since it does not contain tables of the
    ///  * used modules(as explained above). See NetCoreMigrationsDbContext for migrations.
    ///  */
    /// </summary>
    [ConnectionStringName(CommonConsts.PgSqlConnectionStringName)]
    public class NetDbContext : AbpDbContext<NetDbContext>
    {
        #region 租户公共数据结构
        public DbSet<EnvironmentRecord> EnvironmentRecords { get; set; }
        public DbSet<CraneAlarm> CraneAlarms { get; set; }
        public DbSet<CraneRecord> CraneRecords { get; set; }
        public DbSet<CraneBasic> CraneBasics { get; set; }

        #endregion


        public NetDbContext(DbContextOptions<NetDbContext> options) : base(options)
        {
            Serilog.Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(NetDbContext)} DbContextOptions.............");
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder.IsUsingPostgreSql())
            {
                Serilog.Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、Tenant使用的PostGreSql");
            }
            if (builder.IsUsingMySQL())
            {
                Serilog.Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、Tenant使用的MySQL");
            }
            if (builder.IsUsingSqlServer())
            {
                Serilog.Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、Tenant使用的SqlServer");
            }
            Serilog.Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(NetDbContext)} OnModelCreating start.............");


            base.OnModelCreating(builder);
            builder.Configure(true);

            Serilog.Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、This is {nameof(NetDbContext)} OnModelCreating end.............");
        }
    }
}