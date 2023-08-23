namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 授权回调dto
    /// </summary>
    public class WeixinAuthReturnDto
    {
        /// <summary>
        /// code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// state
        /// </summary>
        public string State { get; set; }

        ///// <summary>
        ///// 时间戳 state 转为时间戳
        ///// </summary>
        //public long? Tickes { get; set; }
    }
}
