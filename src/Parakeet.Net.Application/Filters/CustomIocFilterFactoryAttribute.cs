using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Parakeet.Net.Filters
{
    /// <summary>
    /// 自定义属性 实现IFilterFactory 完成filter的依赖注入
    /// </summary>
    public class CustomIocFilterFactoryAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => true;

        private readonly Type _filterType = null;
        public CustomIocFilterFactoryAttribute(Type type)
        {
            _filterType = type;//这个type是交给容器创建的，但是CustomIocFilterFactoryAttribute本身是反射后由工厂创建的实例GetCustomAttribute
        }

        /// <summary>
        /// 创建filter实例
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return (IFilterMetadata)serviceProvider.GetService(this._filterType);
        }

    }
}
