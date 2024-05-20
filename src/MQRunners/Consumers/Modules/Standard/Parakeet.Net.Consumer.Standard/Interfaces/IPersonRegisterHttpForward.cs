using Common.Dtos;
using Parakeet.Net.Consumer.Interfaces;

namespace Parakeet.Net.Consumer.Standard.Interfaces
{
    /// <summary>
    /// 注册人员接口
    /// </summary>
    public interface IPersonRegisterHttpForward : IHttpForward<DeviceWorkerDto>
    {
    }
}
