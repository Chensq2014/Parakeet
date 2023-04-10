using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Parakeet.Net.Interfaces;

namespace Parakeet.Net.Extensions
{
    /// <summary>
    /// 向容器中获取注册接口或类的实例
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// GetRequiredService-->Resolve
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static T Resolve<T>(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<T>();
        }

        /// <summary>
        /// 一个接口注册了多个实现，通过HandlerType来获取唯一实现类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <param name="handlerType"></param>
        /// <returns></returns>
        public static T Resolve<T>(this IServiceProvider serviceProvider, string handlerType) where T : IHandlerType
        {
            return serviceProvider.GetServices<T>().FirstOrDefault(s => s.HandlerType.Equals(handlerType, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
