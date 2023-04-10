using System;
using Parakeet.Net.Enums;
using System.ComponentModel;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace Parakeet.Net.Events
{
    /// <summary>
    /// 读消息事件 更新用户未读消息数 使用事件还不如直接更新
    /// </summary>
    [Serializable]
    public class ReadNotifyEvent : EtoBase// EventData
    {
        public ReadNotifyEvent(Guid toUserId, int count, NotifyType notifyType = NotifyType.系统消息)
        {
            ToUserId = toUserId;
            NotifyCount = count;
            NotifyType = notifyType;
        }

        /// <summary>
        /// 读取消息数目 负数时表示标记为未读
        /// </summary>
        [Description("消息数目")]
        public int NotifyCount { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public NotifyType NotifyType { get; set; }

        /// <summary>
        /// 接收者
        /// </summary>
        [Description("接收者")]
        public Guid ToUserId { get; set; }
    }
}
