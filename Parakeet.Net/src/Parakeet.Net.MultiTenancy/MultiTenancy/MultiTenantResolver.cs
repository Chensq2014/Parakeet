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
    ///  替换默认实现 ITenantResolver, ITransientDependency
    /// </summary>
    [Dependency(ReplaceServices = true)]
    public class MultiTenantResolver : TenantResolver
    {
        public MultiTenantResolver(IOptions<AbpTenantResolveOptions> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
        }

        public override Task<TenantResolveResult> ResolveTenantIdOrNameAsync()
        {
            //父类代码拷贝出来调试
            return base.ResolveTenantIdOrNameAsync();
        }
    }
}
