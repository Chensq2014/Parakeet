using Common.Entities;
using Common.Interfaces;
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
