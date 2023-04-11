using System;

namespace Parakeet.Net.Test
{
    /// <summary>
    /// 自定义容器接口 用于替换 IServiceCollection
    /// </summary>
    public interface IParakeetContainer
    {
        /// <summary>
        /// 注册接口与类型
        /// </summary>
        /// <typeparam name="IService"></typeparam>
        /// <typeparam name="Service"></typeparam>
        void Register<IService, Service>() where Service : IService;

        /// <summary>
        /// 注册接口与实现类
        /// </summary>
        /// <param name="type"></param>
        /// <param name="implType"></param>
        void Register(Type type, Type implType);

        /// <summary>
        /// 根据类型获取
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        object Resolve(Type type);

        IServiceProvider BuildServiceProvider();
    }
}
