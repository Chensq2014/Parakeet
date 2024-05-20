using Parakeet.Net.Samples;
using Xunit;

namespace Parakeet.Net.EntityFrameworkCore.Domains;

[Collection(NetTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<NetEntityFrameworkCoreTestModule>
{

}
