using Parakeet.Net.Entities;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.Thresholds
{
    /// <summary>
    /// 阈值管理
    /// </summary>
    public interface IThresholdAppService : IBaseNetAppService<Threshold>, ITransientDependency
    {

    }
}
