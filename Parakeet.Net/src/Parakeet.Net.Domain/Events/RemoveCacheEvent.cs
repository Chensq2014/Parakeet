using System;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus;

namespace Parakeet.Net.Events
{
    /// <summary>
    /// 根据key移除Redis Cache事件
    /// </summary>
    [Serializable]
    public class RemoveCacheEvent: EtoBase//EventData
    {
        public RemoveCacheEvent(string cacheName, string key = "")
        {
            CacheName = cacheName;
            Key = key;
        }

        /// <summary>
        /// 缓存组名
        /// </summary>
        public string CacheName { get; set; }

        /// <summary>
        /// 缓存键Key
        /// </summary>
        public string Key { get; set; }
    }
}
