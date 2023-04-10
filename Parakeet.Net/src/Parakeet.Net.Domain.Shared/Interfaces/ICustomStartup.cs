using Microsoft.Extensions.DependencyInjection;
using System;

namespace Parakeet.Net.Interfaces
{
    /// <summary>
    /// 自定义启动类接口
    /// </summary>
    public interface ICustomStartup
    {
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="services"></param>
        void ConfigureServices(IServiceCollection services);

        /// <summary>
        /// 配置管道/启动执行顺序
        /// </summary>
        void Configure(IServiceProvider serviceProvider);

        /// <summary>
        /// 启动类所属模块
        /// </summary>
        ICustomModule Module { get; }
    }
}
