using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nacos.V2.Naming.Dtos;
using Parakeet.Net.Data;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.EntityFrameworkCore;

/// <summary>
/// 生成数据库时使用
/// </summary>
public class CommonSchemaMigrator
    : INetDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public CommonSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the NetDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            //.GetRequiredService<PortalDbContext>()//公共数据库
            //.GetRequiredService<PgSqlMigrationsDbContext>()//租户-PgSql数据库
            //.GetRequiredService<MySqlMigrationsDbContext>()//租户-MySql数据库
            //.GetRequiredService<SqlServerMigrationsDbContext>()//租户-SqlServer数据库
            .Database
            .MigrateAsync();
    }
}
