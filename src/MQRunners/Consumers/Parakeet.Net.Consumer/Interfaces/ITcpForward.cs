using Common.Dtos;

namespace Parakeet.Net.Consumer.Interfaces
{
    /// <summary>
    /// Tcp转发基类接口
    /// </summary>
    /// <typeparam name="TDeviceRecordDto"></typeparam>
    public interface ITcpForward<TDeviceRecordDto> : IForward<TDeviceRecordDto> where TDeviceRecordDto : DeviceRecordDto
    {

    }
}
