using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Parakeet.Net.MultiTenancy
{
    /// <summary>
    ///  替换默认实现 ITenantConfigurationProvider, ITransientDependency
    /// </summary>
    [Dependency(ReplaceServices = true)]
    public class MultiTenantConfigurationProvider : TenantConfigurationProvider
    {
        public MultiTenantConfigurationProvider(ITenantResolver tenantResolver, ITenantStore tenantStore, ITenantResolveResultAccessor tenantResolveResultAccessor) : base(tenantResolver, tenantStore, tenantResolveResultAccessor)
        {
        }

        protected override Task<TenantConfiguration> FindTenantAsync(string tenantIdOrName)
        {
            //查看父类源码，拷贝出来调试
            return base.FindTenantAsync(tenantIdOrName);
        }
    }
}
