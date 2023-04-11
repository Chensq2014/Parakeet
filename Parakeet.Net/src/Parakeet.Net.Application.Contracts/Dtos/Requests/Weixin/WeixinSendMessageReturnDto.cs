namespace Parakeet.Net.Dtos.Requests
{
    public class WeixinSendMessageReturnDto: WebClientResultBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string Invaliduser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Invalidparty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Invalidtag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Unlicenseduser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Msgid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Response_code { get; set; }
    }
}
