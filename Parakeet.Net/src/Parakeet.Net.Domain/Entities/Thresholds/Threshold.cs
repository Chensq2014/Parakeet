using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Parakeet.Net.Entities.Devices;
using Parakeet.Net.Enums;

namespace Parakeet.Net.Entities.Thresholds
{
    /// <summary>
    /// 阈值
    /// </summary>
    [Description("阈值")]
    [Table("Parakeet_Thresholds", Schema = "parakeet")]
    public class Threshold : BaseEntity
    {
        public Threshold() { }

        public Threshold(Guid id)
        {
            SetEntityPrimaryKey(id);
        }

        #region 基础信息

        /// <summary>
        /// 设备名称
        /// </summary>
        [Description("设备名称")]
        [MaxLength(CustomerConsts.MaxLength255)]
        public string Name { set; get; }

        /// <summary>
        /// 设备类型
        /// </summary>
        [Description("设备类型")]
        public DeviceType DeviceType { get; set; }

        /// <summary>
        /// 报警最小值
        /// </summary>
        [Description("报警最小值")]
        public decimal? MinAlarmValue { set; get; }

        /// <summary>
        /// 报警最大值
        /// </summary>
        [Description("报警最大值")]
        public decimal? MaxAlarmValue { set; get; }

        /// <summary>
        /// 预警最小值
        /// </summary>
        [Description("预警最小值")]
        public decimal? MinWarningValue { set; get; }

        /// <summary>
        /// 预警最大值
        /// </summary>
        [Description("预警最大值")]
        public decimal? MaxWarningValue { set; get; }

        /// <summary>
        /// 版本号
        /// </summary>
        [Description("版本号")]
        public decimal? Version { set; get; }

        /// <summary>
        /// Factor 形象因素(字段名称)
        /// </summary>
        [Description("Factor 形象因素(字段名称)")]
        [MaxLength(CustomerConsts.MaxLength255)]
        public string Factor { get; set; }

        #endregion

        #region 设备

        /// <summary>
        /// 关联的设备
        /// </summary>
        public virtual ICollection<Device> Devices { get; set; } = new HashSet<Device>();

        #endregion
    }
}
