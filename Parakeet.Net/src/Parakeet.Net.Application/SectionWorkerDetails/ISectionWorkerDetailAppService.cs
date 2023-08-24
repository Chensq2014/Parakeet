using Parakeet.Net.Entities;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.SectionWorkerDetails
{
    /// <summary>
    /// 区域工人工作明细
    /// </summary>
    public interface ISectionWorkerDetailAppService : IBaseNetAppService<SectionWorkerDetail>, ITransientDependency
    {
        
    }
}