using Volo.Abp.Modularity;

namespace Parakeet.Net;

[DependsOn(
    typeof(NetApplicationModule),
    typeof(NetDomainTestModule)
    )]
public class NetApplicationTestModule : AbpModule
{

}
