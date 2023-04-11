using Parakeet.Net.Dtos;

namespace Parakeet.Net.Devices.Dtos
{
    /// <summary>
    /// 转发错误类
    /// </summary>
    public class ForwardErrorRecordDto: DeviceRecordDto
    {
        /// <summary>
        /// 返回码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 转发区域
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 转发内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount { get; set; } = 0;
    }
}
