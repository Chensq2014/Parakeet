using Common;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Parakeet.Net.ConsoleApp;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(NetApplicationModule)
    )]
public class NetConsoleModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
    }
}
