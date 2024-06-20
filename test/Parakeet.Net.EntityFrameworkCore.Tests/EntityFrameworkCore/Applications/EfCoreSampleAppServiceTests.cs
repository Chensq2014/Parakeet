using Parakeet.Net.Samples;
using Xunit;

namespace Parakeet.Net.EntityFrameworkCore.Applications;

[Collection(NetTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<NetEntityFrameworkCoreTestModule>
{

}
