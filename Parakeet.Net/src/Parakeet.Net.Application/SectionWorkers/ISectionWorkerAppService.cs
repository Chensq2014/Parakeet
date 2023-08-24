using Parakeet.Net.Entities;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.SectionWorkers
{
    /// <summary>
    /// 区域工人服务
    /// </summary>
    public interface ISectionWorkerAppService : IBaseNetAppService<SectionWorker>, ITransientDependency
    {
        
    }
}