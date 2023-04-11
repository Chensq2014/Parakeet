namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 企业微信自建/三方应用获取sdk传输dto
    /// </summary>
    public class WeixinSdkSignatureInputDto
    {
        /// <summary>
        /// noncestr
        /// </summary>
        public string Noncestr { get; set; }

        /// <summary>
        /// timestamp
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 是否获取企业ticket,默认获取企业应用ticket(false)
        /// </summary>
        public bool IsEnterPrise { get; set; }
    }
}
