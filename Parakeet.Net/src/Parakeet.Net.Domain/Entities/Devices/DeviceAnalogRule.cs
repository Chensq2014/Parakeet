using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 数据模拟规则
    /// </summary>
    [Description("数据模拟规则")]
    [Table("Parakeet_DeviceAnalogRules", Schema = "parakeet")]
    public class DeviceAnalogRule :  BaseEntity
    {
        public DeviceAnalogRule()
        {
        }

        public DeviceAnalogRule(Guid id)
        {
            base.SetEntityPrimaryKey(id);
        }

        /// <summary>
        /// 设备Id
        /// </summary>
        [Required, Description("设备Id")]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        public virtual Device Device { get; set; }

        /// <summary>
        /// 发送数据频率 时间间隔
        /// </summary>
        [Required, Description("频率/时间间隔")]
        public TimeSpan Period { get; set; }

        /// <summary>
        /// 最后一次发送数据时间
        /// </summary>
        [Required, Description("最后一次发送数据时间")]
        public DateTime LastSendTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 是否启用规则  默认为false禁用状态
        /// </summary>
        [Description("是否启用状态  默认为false")]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 扩展一个调用Url地址
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength512), Description("扩展Url地址")]
        public string ExtendUrl { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength4096), Description("备注")]
        public string Remark { get; set; }
    }
}
