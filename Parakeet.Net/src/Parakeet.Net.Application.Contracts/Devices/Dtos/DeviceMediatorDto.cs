using System;
using System.ComponentModel;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 设备转发器
    /// </summary>
    public class DeviceMediatorDto : BaseDto
    {

        #region 基础字段

        /// <summary>
        /// 是否转发
        /// </summary>
        [Description("是否转发")]
        public bool Forward { get; set; }

        /// <summary>
        /// 是否持久化
        /// </summary>
        [Description("是否持久化")]
        public bool Persist { get; set; }

        #endregion

        #region 设备

        [Description("设备Id")]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        public virtual DeviceDto Device { get; set; }

        #endregion

        #region 转发器

        /// <summary>
        /// 转发器Id
        /// </summary>
        [Description("转发器Id")]
        public Guid MediatorId { get; set; }

        /// <summary>
        /// 转发器
        /// </summary>
        [Description("转发器")]
        public virtual MediatorDto Mediator { get; set; }

        #endregion
    }
}
