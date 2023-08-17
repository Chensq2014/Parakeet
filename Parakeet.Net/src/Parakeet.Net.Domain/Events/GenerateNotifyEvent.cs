using System;
using System.Collections.Generic;
using System.ComponentModel;
using Parakeet.Net.Entities;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace Parakeet.Net.Events
{
    /// <summary>
    /// 产生一条消息通知
    /// </summary>
    [Serializable]
    public class GenerateNotifyEvent : EtoBase//: EventData
    {
        public GenerateNotifyEvent(IEnumerable<Notify> notifies)
        {
            Notifies = notifies;
        }

        /// <summary>
        /// 消息数目
        /// </summary>
        [Description("消息数目")]
        public IEnumerable<Notify> Notifies { get; set; }
    }
}
