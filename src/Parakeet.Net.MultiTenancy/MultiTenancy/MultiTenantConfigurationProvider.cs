using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.MultiTenancy.Localization;

namespace Parakeet.Net.MultiTenancy
{
    /// <summary>
    ///  替换默认实现 ITenantConfigurationProvider, ITransientDependency
    /// </summary>
    [Dependency(ReplaceServices = true)]
    public class MultiTenantConfigurationProvider : TenantConfigurationProvider
    {
        public MultiTenantConfigurationProvider(ITenantResolver tenantResolver, ITenantStore tenantStore, ITenantResolveResultAccessor tenantResolveResultAccessor,IStringLocalizer<AbpMultiTenancyResource> stringLocalizer, ITenantNormalizer tenantNormalizer) 
            : base(tenantResolver, tenantStore, tenantResolveResultAccessor,stringLocalizer, tenantNormalizer)
        {
        }

        protected override Task<TenantConfiguration> FindTenantAsync(string tenantIdOrName)
        {
            //查看父类源码，拷贝出来调试
            return base.FindTenantAsync(tenantIdOrName);
        }
    }
}
