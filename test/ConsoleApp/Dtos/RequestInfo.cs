namespace ConsoleApp.Dtos
{
    /// <summary>
    /// 请求对象
    /// </summary>
    public class RequestInfo
    {
        /// <summary>
        /// 供应商appid
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 供应商appSecret
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// json数据字符串
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 格式，json
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// 接口标识符，比如upload.envMonitor
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 随机数
        /// </summary>
        public string Nonce { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// 版本，比如1.0
        /// </summary>
        public string Version { get; set; }
    }
}
