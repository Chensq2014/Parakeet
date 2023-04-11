namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 获取AppToken输入类
    /// </summary>
    public class AppTokenInputDto
    {
        /// <summary>
        /// appId
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// appkey
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// appSecret
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// unix时间戳
        /// </summary>
        public string TimeStamp { get; set; }

        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; }
    }
}
