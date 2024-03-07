using Volo.Abp.Modularity;

namespace Parakeet.Net;

/* Inherit from this class for your domain layer tests. */
public abstract class NetDomainTestBase<TStartupModule> : NetTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
