using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Common;

namespace Parakeet.Net.EntityFrameworkCore
{
    /* This class is needed for EF Core console commands
     * (like Add-Migration and Update-Database commands) */
    public class NetDbContextFactory : IDesignTimeDbContextFactory<NetDbContext>
    {
        public NetDbContext CreateDbContext(string[] args)
        {
            var configuration = BuildConfiguration();
            //UseSqlServer UseNpgsql  UseMySql
            var builder = new DbContextOptionsBuilder<NetDbContext>()
                .UseNpgsql(configuration.GetConnectionString(CommonConsts.MultiTenantConnectionStringName));
                //.UseLoggerFactory(new SerilogLoggerFactory());
                ////这会使所有查询都不被跟踪。 仍可添加 AsTracking() 来进行特定查询跟踪。
                //.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            return new NetDbContext(builder.Options);
        }

        /// <summary>
        /// 获取根目录IConfigurationRoot
        /// </summary>
        /// <returns></returns>
        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                //.SetBasePath(Directory.GetCurrentDirectory())
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Parakeet.Net.DbMigrator/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
