namespace Parakeet.Net.Dtos.Emails
{
    /// <summary>
    /// smtp配置项
    /// </summary>
    public class SetSmtpConfigDto : DomainConfigDto
    {
        //密码 默认 1qaz2WSX3edc
        public string Password { get; set; } = "1qaz2WSX3edc";
    }
}
