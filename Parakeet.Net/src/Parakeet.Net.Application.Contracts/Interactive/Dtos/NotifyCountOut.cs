namespace Parakeet.Net.Interactive.Dtos
{
    /// <summary>
    /// 用户未读消息数据
    /// </summary>
    public class NotifyCountOut
    {
        /// <summary>
        /// 未读消息总数
        /// </summary>
        public int NotifyCount => SystemNotifyCount + ApplicationNotifyCount;
        /// <summary>
        /// 系统消息数
        /// </summary>
        public int SystemNotifyCount { get; set; }
        /// <summary>
        /// 应用消息数
        /// </summary>
        public int ApplicationNotifyCount { get; set; }
    }
}
