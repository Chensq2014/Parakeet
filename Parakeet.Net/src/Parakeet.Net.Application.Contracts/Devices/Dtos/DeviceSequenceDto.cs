using Parakeet.Net.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 某区域某类型设备编号自增
    /// </summary>
    public class DeviceSequenceDto : BaseDto
    {

        #region 基础字段

        /// <summary>
        /// 某具体区域
        /// </summary>
        [Required]
        [MaxLength(CustomerConsts.MaxLength16)]
        [Description("某具体区域")]
        public string Area { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        [Required]
        [Range(1000, 9999)]
        [Description("设备类型")]
        public DeviceType DeviceType { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [Description("序号")]
        public long Sequence { get; set; }

        #endregion

        #region 设备

        /// <summary>
        /// 设备Id
        /// </summary>
        [Required]
        [Description("设备Id")]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        public virtual DeviceDto Device { get; set; }

        #endregion
    }
}
