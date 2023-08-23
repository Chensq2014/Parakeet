using Parakeet.Net.Entities;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.Products
{
    /// <summary>
    /// 产品服务
    /// </summary>
    public interface IProductAppService : IBaseNetAppService<Product>, ITransientDependency
    {
        
    }
}