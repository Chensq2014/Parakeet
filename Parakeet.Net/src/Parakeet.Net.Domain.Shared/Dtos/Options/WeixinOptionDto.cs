namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 企业微信配置类
    /// </summary>
    public class WeixinOptionDto : BaseAppOptionDto
    {
        public const string ConfigKey = "App:MicroServices:Weixin";

        /// <summary>
        /// 应用Id【自建/三方】
        /// </summary>
        public string AgentId { get; set; }

    }
}
