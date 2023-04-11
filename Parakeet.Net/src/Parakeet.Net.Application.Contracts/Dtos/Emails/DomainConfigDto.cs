namespace Parakeet.Net.Dtos.Emails
{
    /// <summary>
    /// domain配置项
    /// </summary>
    public class DomainConfigDto
    {
        /// <summary>
        /// 域名 默认shuangquan.aksoinfo.com
        /// </summary>
        public string DomainName { get; set; } = "shuangquan.aksoinfo.com";

        /// <summary>
        /// 域名Id 创建域名时可不传递
        /// </summary>
        public int? DomainId { get; set; }
    }
}
