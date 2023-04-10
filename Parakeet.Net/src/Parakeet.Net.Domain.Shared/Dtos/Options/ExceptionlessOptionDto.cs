namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// Exceptionless配置类
    /// </summary>
    public class ExceptionlessOptionDto : BaseAppOptionDto
    {
        /// <summary>
        /// Exceptionless ApiKey
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Exceptionless server服务端地址
        /// </summary>
        public string ServerUrl { get; set; }
    }
}
