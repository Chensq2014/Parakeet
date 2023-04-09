namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 宇泛设置任务返回结果
    /// </summary>
    public class ExecuteTaskResultDto<T> where T : class
    {
        /// <summary>
        /// 设备型号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 处理结果 1
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 返回Data数据
        /// </summary>
        public T Data { get; set; }
    }

    /// <summary>
    /// 默认返回data为字符串
    /// </summary>
    public class ExecuteTaskResultDto : ExecuteTaskResultDto<string>
    {
    }
}