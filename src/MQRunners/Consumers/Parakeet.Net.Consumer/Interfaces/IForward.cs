using Common.Dtos;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Parakeet.Net.Consumer.Interfaces
{
    /// <summary>
    /// 转发器
    /// </summary>
    public interface IForward<TDeviceRecordDto> where TDeviceRecordDto : DeviceRecordDto
    {
        /// <summary>
        /// 转发处理方法
        /// </summary>
        /// <param name="wrapperData">数据源</param>
        /// <param name="content">httpoCntent</param>
        /// <returns></returns>
        Task Push(WrapperData<TDeviceRecordDto> wrapperData, [CanBeNull] HttpContent content = null);

        /// <summary>
        /// 批量转发
        /// </summary>
        /// <param name="wrapperDataList">封装的数据源集合</param>
        /// <param name="maximumConcurrent">最大并发推送数量，默认为50</param>
        /// <param name="maximumMilliseconds">最大等待时间（毫秒），超过此时间后不再等待所有请求结果，默认为10秒</param>
        /// <returns></returns>
        Task BatchPush(List<WrapperData<TDeviceRecordDto>> wrapperDataList, int maximumConcurrent = 50, int maximumMilliseconds = 10 * 1000);
    }
}
