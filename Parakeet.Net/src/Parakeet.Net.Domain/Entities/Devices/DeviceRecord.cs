using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 设备记录的父类
    /// </summary>
    [Description("设备记录父类")]
    public class DeviceRecord : BaseEntity
    {
        public DeviceRecord()
        {
        }

        public DeviceRecord(Guid id)
        {
            base.SetEntityPrimaryKey(id);
        }

        /// <summary>
        /// 设备Id
        /// </summary>
        [Description("设备Id")]
        public Guid? DeviceId { get; set; }

        /// <summary>
        /// 记录采集时间
        /// </summary>
        [Required]
        [Description("记录采集时间")]
        public DateTime RecordTime { get; set; }
    }
}
