using Microsoft.Extensions.Options;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;

namespace Parakeet.Net.Services
{
    /// <summary>
    /// 领域层Services基类 暂无公共基础扩展
    /// </summary>
    public class NetDomainServiceBase : DomainService
    {
        //protected IOptions<AbpSequentialGuidGeneratorOptions> SequentialGuidGeneratorOptions => LazyServiceProvider.LazyGetService<IOptions<AbpSequentialGuidGeneratorOptions>>();
        //protected IGuidGenerator SequentialGuidGenerator => LazyServiceProvider.LazyGetService<IGuidGenerator>(new SequentialGuidGenerator(SequentialGuidGeneratorOptions));

    }
}
