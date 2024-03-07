using Volo.Abp.Modularity;

namespace Parakeet.Net;

public abstract class NetApplicationTestBase<TStartupModule> : NetTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
