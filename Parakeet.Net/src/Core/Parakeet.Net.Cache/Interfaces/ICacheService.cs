using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.Cache
{
    /// <summary>
    /// 缓存实体类型properties服务接口
    /// </summary>
    public interface ICacheService : ITransientDependency
    {
        /// <summary>
        /// 缓存实体类型的 所有可用属性/字段(未被特殊属性标记：如BaseField)
        /// </summary>
        /// <param name="type"></param>
        /// <returns>返回当前缓存的实体属性集合</returns>
        Task<List<PropertyInfo>> GetCachePropertyInfos(Type type);

        /// <summary>
        /// 缓存类型的 所有可用属性/字段名称(未被特殊属性标记：如BaseField)
        /// </summary>
        /// <param name="type"></param>
        /// <returns>返回实体列名集合</returns>
        Task<List<string>> GetCachePropNames(Type type);
    }
}
