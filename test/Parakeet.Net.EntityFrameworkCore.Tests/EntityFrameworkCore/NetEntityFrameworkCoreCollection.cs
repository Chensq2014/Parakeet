using Xunit;

namespace Parakeet.Net.EntityFrameworkCore;

[CollectionDefinition(NetTestConsts.CollectionDefinitionName)]
public class NetEntityFrameworkCoreCollection : ICollectionFixture<NetEntityFrameworkCoreFixture>
{

}
