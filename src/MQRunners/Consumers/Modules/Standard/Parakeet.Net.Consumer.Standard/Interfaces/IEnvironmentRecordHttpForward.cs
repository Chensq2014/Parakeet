using Common.Dtos;
using Parakeet.Net.Consumer.Interfaces;

namespace Parakeet.Net.Consumer.Standard.Interfaces
{
    /// <summary>
    /// 环境转发接口
    /// </summary>
    public interface IEnvironmentRecordHttpForward : IHttpForward<EnvironmentRecordDto>
    {
    }
}
