namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 获取任务参数
    /// </summary>
    public class GetTaskDto
    {
        /// <summary>
        /// 设备序列号 (真实序列号不能更改)
        /// </summary>
        public string DeviceKey { get; set; }
    }
}