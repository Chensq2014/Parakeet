using Common;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.MultiTenancy.ConfigurationStore;
using Volo.Abp.TenantManagement;
using Volo.Abp.Threading;

namespace Parakeet.Net.MultiTenancy
{
    [Dependency(ReplaceServices = true)]
    public class TenantStore : ITenantStore, ITransientDependency //DefaultTenantStore//,
    {
        private readonly AbpDefaultTenantStoreOptions _options;
        //private ITenantManager _tenantManager;
        private ITenantRepository _tenantRepository;
        private IDistributedCache<TenantConfiguration> _tenantCache;
        public TenantStore(IOptionsMonitor<AbpDefaultTenantStoreOptions> options,
            IDistributedCache<TenantConfiguration> tenantCache, ITenantRepository tenantRepository
            //,ITenantManager tenantManager
            ) //: base(options)
        {
            _tenantCache = tenantCache;
            _tenantRepository = tenantRepository;
            //_tenantManager = tenantManager;
            _options = options.CurrentValue;
        }

        public async Task<TenantConfiguration> FindAsync(string name)
        {
            var tenantConfiguration = await _tenantCache.GetOrAddAsync(name, async () =>
            {
                //_tenantRepository 如有自己租户这里换成自己的租户表或租户dbContext的repository
                var tenant = await _tenantRepository.FindByNameAsync(name);
                var tenantConfiguration = tenant != null
                    ? new TenantConfiguration(tenant.Id, tenant.Name)
                    : null;
                tenantConfiguration?.ConnectionStrings.TryAdd(tenantConfiguration.ConnectionStrings.Default, tenant.FindDefaultConnectionString());
                return tenantConfiguration;
            });
            return tenantConfiguration;
        }

        public async Task<TenantConfiguration> FindAsync(Guid id)
        {
            var tenantConfiguration = await _tenantCache.GetOrAddAsync(id.ToString(), async () =>
            {
                var tenant = await _tenantRepository.FindAsync(id);
                var tenantConfiguration = tenant != null
                    ? new TenantConfiguration(tenant.Id, tenant.Name)
                    : null;
                object value = tenantConfiguration?.ConnectionStrings.TryAdd(tenantConfiguration.ConnectionStrings.Default ?? CommonConsts.DefaultConnectStringName, tenant.FindDefaultConnectionString());
                return tenantConfiguration;
            });
            return tenantConfiguration;
        }

        public TenantConfiguration Find(string name)
        {
            return AsyncHelper.RunSync(async () => await FindAsync(name));
        }

        public TenantConfiguration Find(Guid id)
        {
            return AsyncHelper.RunSync(async () => await FindAsync(id));
        }

        public async Task<IReadOnlyList<TenantConfiguration>> GetListAsync(bool includeDetails)
        {
            var tenants = await _tenantRepository.GetListAsync();
            return (IReadOnlyList<TenantConfiguration>)tenants.Select(x => new TenantConfiguration
            {
                Id = x.Id,
                Name = x.Name
            });
        }
    }
}
