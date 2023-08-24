using Parakeet.Net.Entities;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.WorkerTypes
{
    /// <summary>
    /// 工人服务
    /// </summary>
    public interface IWorkerAppService : IBaseNetAppService<Worker>, ITransientDependency
    {
        
    }
}