using Common.Devices.Dtos;
using Common.Dtos;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Parakeet.Net.Consumer.Interfaces
{
    /// <summary>
    /// Http转发基类接口
    /// </summary>
    /// <typeparam name="TDeviceRecordDto"></typeparam>
    public interface IHttpForward<TDeviceRecordDto> : IForward<TDeviceRecordDto> where TDeviceRecordDto : DeviceRecordDto
    {
        /// <summary>
        /// 创建HttpClient
        /// </summary>
        /// <returns>HttpClient</returns>
        Task<HttpClient> CreateClient();

        /// <summary>
        /// 初始化转发配置【ForwardConfigDto】虽然配置文件已默认，
        /// 但提供这个初始化方法可在子类构造函数中设置子类的默认值
        /// </summary>
        /// <param name="configAction"></param>
        /// <returns></returns>
        Task Init(Action<ForwardConfigDto> configAction);
    }
}
