using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 设备记录父类
    /// </summary>
    public class DeviceRecordDto : BaseDto//FullAuditedEntityDto<Guid>
    {
        #region 设备
        /// <summary>
        /// 设备Id
        /// </summary>
        [Description("设备Id")]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        public virtual DeviceDto Device { get; set; }

        #endregion

        /// <summary>
        /// 记录采集时间
        /// </summary>
        public DateTime RecordTime { get; set; }
    }
}
