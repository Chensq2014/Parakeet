namespace Parakeet.Net.Dtos.Requests
{
    /// <summary>
    /// 企业微信自建/三方应用获取微信api返回dto
    /// </summary>
    public class WeixinSdkSignatureReturnDto : WeixinSdkSignatureInputDto
    {
        ///// <summary>
        ///// token
        ///// </summary>
        //public string Jsapi_ticket { get; set; }
        
        /// <summary>
        /// 唯一表示
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 应用Id【自建/三方】
        /// </summary>
        public string AgentId { get; set; }

        /// <summary>
        /// 签名信息
        /// </summary>
        public string Signature { get; set; }
    }
}
