namespace Parakeet.Net.Dtos.Tcp
{
    /// <summary>
    /// tcp配置文件反射类
    /// </summary>
    public class TcpOptionDto
    {
        /// <summary>
        /// 主机
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// 最大池
        /// </summary>
        public int MaxPool { get; set; }

    }
}
