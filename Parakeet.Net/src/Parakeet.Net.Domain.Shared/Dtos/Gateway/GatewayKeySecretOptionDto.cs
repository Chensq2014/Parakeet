namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 网关配置
    /// </summary>
    public class GatewayKeySecretOptionDto : BaseAppOptionDto
    {
        /// <summary>
        /// 公钥
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// 私钥
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// 网关地址/grpc server服务端地址
        /// </summary>
        public string ServerUrl { get; set; }
    }
}
