using System;

namespace Parakeet.Net.Factory
{
    /// <summary>
    /// 静态工厂
    /// </summary>
    public static class ConsoleServiceProviderFactory
    {
        /// <summary>
        /// 实例提供器：去启动模块给它赋初值，这样它就可以任意时候为上层提供实例了
        /// 适用于console程序
        /// </summary>
        public static IServiceProvider ServiceProvider { get; set; }
        static ConsoleServiceProviderFactory()
        {
        }
    }
}
