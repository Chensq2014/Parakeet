using Parakeet.Net.Entities;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.Sections
{
    /// <summary>
    /// 小区区域服务
    /// </summary>
    public interface ISectionAppService : IBaseNetAppService<Section>, ITransientDependency
    {
        
    }
}