using System;
using System.ComponentModel;

namespace Parakeet.Net.Dtos.DeviceAnalogRules
{
    /// <summary>
    /// 获取规则数据列表
    /// </summary>
    public class GetDeviceAnalogRuleBaseInput : PagedInputDto
    {
        /// <summary>
        /// 项目Id
        /// </summary>
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>
        public Guid? DeviceId { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 是否启用规则  默认为false禁用状态
        /// </summary>
        [Description("是否启用状态  默认为false")]
        public bool? IsEnabled { get; set; }
    }
}
