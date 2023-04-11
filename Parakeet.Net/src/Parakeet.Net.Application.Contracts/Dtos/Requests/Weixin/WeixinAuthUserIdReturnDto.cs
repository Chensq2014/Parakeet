namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 企业微信临时授权Code返回用户dto
    /// </summary>
    public class WeixinAuthUserIdReturnDto : WebClientResultBase
    {

        /// <summary>
        /// 用户信息
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 企业OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 成员票据，最大为512字节，有效期为1800s。
        /// scope为snsapi_privateinfo，且用户在应用可见范围之内时返回此参数。
        /// 后续利用该参数可以获取用户信息或敏感信息，参见"获取访问用户敏感信息"。暂时不支持上下游或/企业互联场景
        /// </summary>
        public string User_ticket { get; set; }

        /// <summary>
        /// 外部联系人id，当且仅当用户是企业的客户，且跟进人在应用的可见范围内时返回。
        /// 如果是第三方应用调用，针对同一个客户，同一个服务商不同应用获取到的id相同
        /// </summary>
        public string External_userid { get; set; }


    }
}
