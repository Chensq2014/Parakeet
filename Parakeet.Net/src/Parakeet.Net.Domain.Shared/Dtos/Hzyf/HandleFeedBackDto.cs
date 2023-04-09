namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 处理任务返回结果
    /// </summary>
    public class HandleFeedBackDto
    {
        /// <summary>
        /// 设备序列号 (真实序列号不能更改)
        /// </summary>
        public string DeviceKey { get; set; }

        /// <summary>
        /// 任务编号
        /// </summary>
        public string TaskNo { get; set; }

        /// <summary>
        /// 处理结果
        /// </summary>
        public string Result { get; set; }
    }
}