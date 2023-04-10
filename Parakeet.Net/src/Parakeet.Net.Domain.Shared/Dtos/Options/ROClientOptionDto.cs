namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// ROClientOptionDto
    /// </summary>
    public class ROClientOptionDto : BaseAppOptionDto
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
        /// grpc server服务端地址
        /// </summary>
        public string ServerUrl { get; set; }
    }
}
