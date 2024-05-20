using Common.Dtos;

namespace Parakeet.Net.ServiceGroup.AlibabaSdk
{
    /// <summary>
    /// 阿里巴巴Sdk配置项
    /// </summary>
    public class AlibabaSdkOption : BaseAppOptionDto
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
        /// 网站服务端地址
        /// </summary>
        public string ServerUrl { get; set; }
    }
}
