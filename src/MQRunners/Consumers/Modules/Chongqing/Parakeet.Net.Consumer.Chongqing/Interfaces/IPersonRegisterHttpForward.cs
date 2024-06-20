using Common.Dtos;
using Parakeet.Net.Consumer.Interfaces;

namespace Parakeet.Net.Consumer.Chongqing.Interfaces
{
    /// <summary>
    /// 注册人员接口
    /// </summary>
    public interface IPersonRegisterHttpForward : IHttpForward<DeviceWorkerDto>
    {
    }
}
