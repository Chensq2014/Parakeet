using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Parakeet.Net.MultiTenancy
{
    /// <summary>
    /// 替换默认实现 IConnectionStringResolver, ITransientDependency
    /// </summary>
    [Dependency(ReplaceServices = true)]
    public class TenantConnectionStringResolver : DefaultConnectionStringResolver
    {
        private readonly ICurrentTenant _currentTenant;
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// 属性注入试试
        /// </summary>
        public ICurrentTenant CurrentTenant { get; set; }
        public Guid? TenentId { get; set; }
        public TenantConnectionStringResolver(
            IOptionsMonitor<AbpDbConnectionOptions> options,
            ICurrentTenant currentTenant,
            IServiceProvider serviceProvider)
            : base(options)
        {
            _currentTenant = currentTenant;
            //CurrentTenant = currentTenant;
            TenentId = _currentTenant.Id;
            _serviceProvider = serviceProvider;
        }

        public override async Task<string> ResolveAsync(string connectionStringName = null)
        {
            if (TenentId == null || (connectionStringName?.Equals(CommonConsts.MutiTenantConnectionStringName) == true))
            {
                //No current tenant, fallback to default logic
                return await base.ResolveAsync(connectionStringName);
            }

            var tenant = await FindTenantConfigurationAsync(TenentId.Value);

            if (tenant == null || tenant.ConnectionStrings.IsNullOrEmpty())
            {
                //Tenant has not defined any connection string, fallback to default logic
                return await base.ResolveAsync(connectionStringName);
            }

            var tenantDefaultConnectionString = tenant.ConnectionStrings.Default;

            //Requesting default connection string...
            if (connectionStringName == null ||
                connectionStringName == ConnectionStrings.DefaultConnectionStringName)
            {
                //Return tenant's default or global default
                return !tenantDefaultConnectionString.IsNullOrWhiteSpace()
                    ? tenantDefaultConnectionString
                    : Options.ConnectionStrings.Default;
            }

            //Requesting specific connection string...
            var connString = tenant.ConnectionStrings.GetOrDefault(connectionStringName);
            if (!connString.IsNullOrWhiteSpace())
            {
                //Found for the tenant
                return connString;
            }

            //Fallback to the mapped database for the specific connection string
            var database = Options.Databases.GetMappedDatabaseOrNull(connectionStringName);
            if (database != null && database.IsUsedByTenants)
            {
                connString = tenant.ConnectionStrings.GetOrDefault(database.DatabaseName);
                if (!connString.IsNullOrWhiteSpace())
                {
                    //Found for the tenant
                    return connString;
                }
            }

            //Fallback to tenant's default connection string if available
            if (!tenantDefaultConnectionString.IsNullOrWhiteSpace())
            {
                return tenantDefaultConnectionString;
            }

            return await base.ResolveAsync(connectionStringName);
        }


        protected virtual async Task<TenantConfiguration> FindTenantConfigurationAsync(Guid tenantId)
        {
            using var serviceScope = _serviceProvider.CreateScope();
            var tenantStore = serviceScope
                .ServiceProvider
                .GetRequiredService<ITenantStore>();

            return await tenantStore.FindAsync(tenantId);
        }

    }
}
