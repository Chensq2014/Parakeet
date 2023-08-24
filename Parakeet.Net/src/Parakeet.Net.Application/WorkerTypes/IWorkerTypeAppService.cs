using Parakeet.Net.Entities;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.WorkerTypes
{
    /// <summary>
    /// 工种服务
    /// </summary>
    public interface IWorkerTypeAppService : IBaseNetAppService<WorkerType>, ITransientDependency
    {
        
    }
}