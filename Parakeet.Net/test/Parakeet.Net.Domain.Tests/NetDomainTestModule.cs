using Parakeet.Net.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Parakeet.Net;

[DependsOn(
    typeof(NetEntityFrameworkCoreTestModule)
    )]
public class NetDomainTestModule : AbpModule
{

}
