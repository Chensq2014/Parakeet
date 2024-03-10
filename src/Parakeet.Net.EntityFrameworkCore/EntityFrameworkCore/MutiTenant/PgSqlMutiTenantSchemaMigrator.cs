using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Parakeet.Net.Data;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.EntityFrameworkCore;

public class PgSqlMutiTenantSchemaMigrator
    : INetDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public PgSqlMutiTenantSchemaMigrator(
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
            .GetRequiredService<NetDbContext>()
            .Database
            .MigrateAsync();
    }
}
