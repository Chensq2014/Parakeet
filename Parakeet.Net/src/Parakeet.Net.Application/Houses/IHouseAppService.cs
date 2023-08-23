using Parakeet.Net.Entities;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.Houses
{
    /// <summary>
    /// 房间服务
    /// </summary>
    public interface IHouseAppService : IBaseNetAppService<House>, ITransientDependency
    {
        
    }
}