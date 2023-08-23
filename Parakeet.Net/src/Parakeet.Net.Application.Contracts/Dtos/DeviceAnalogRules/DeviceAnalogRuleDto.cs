using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 设备模拟数据规则
    /// </summary>
    public class DeviceAnalogRuleDto : BaseDto//EntityDto<Guid>
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        [Description("设备Id")]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [MaxLength(100), Description("设备名称")]
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备编码（对外提供设备序列号）
        /// </summary>
        [Required, MaxLength(50), Description("设备编码")]
        public string FakeNo { get; set; }

        /// <summary>
        /// 设备信息
        /// </summary>
        [Description("设备信息")]
        public virtual DeviceListDto Device { get; set; }

        /// <summary>
        /// 发送数据频率 时间间隔
        /// </summary>
        [Description("频率/时间间隔")]
        public TimeSpan Period { get; set; } = new TimeSpan(1,0,0,0);

        /// <summary>
        /// 最后一次发送数据时间
        /// </summary>
        [Description("最后一次发送数据时间")]
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
