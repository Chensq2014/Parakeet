namespace Parakeet.Net.Consumer.Standard.Dtos
{
    /// <summary>
    /// 请求token返回类型
    /// </summary>
    public class TokenResultDto
    {
        /// <summary>
        /// message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// state
        /// </summary>
        public string State { get; set; }

        public TokenResultDataDto Data { get; set; }
    }
}
