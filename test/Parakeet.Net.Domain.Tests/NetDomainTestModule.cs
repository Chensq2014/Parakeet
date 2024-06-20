using Volo.Abp.Modularity;

namespace Parakeet.Net;

[DependsOn(
    typeof(NetDomainModule),
    typeof(NetTestBaseModule)
)]
public class NetDomainTestModule : AbpModule
{

}
