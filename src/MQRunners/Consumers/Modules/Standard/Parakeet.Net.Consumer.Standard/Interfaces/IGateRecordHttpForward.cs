using Common.Dtos;
using Parakeet.Net.Consumer.Interfaces;

namespace Parakeet.Net.Consumer.Standard.Interfaces
{
    /// <summary>
    /// 闸机考勤转发接口
    /// </summary>
    public interface IGateRecordHttpForward : IHttpForward<GateRecordDto>
    {

    }
}
