using Parakeet.Net.Dtos;
using Parakeet.Net.Interfaces;
using System.Threading.Tasks;

namespace Parakeet.Net.Devices
{
    /// <summary>
    /// 设备反控命令公共接口
    /// </summary>
    public interface IReverseCommand : IHandlerType
    {
        string Name { get; }

        string Area { get; }

        string SupplierCode { get; }

        Task<ResponseWrapper<string>> ExecuteAsync(DeviceDto device, string body);
    }
}
