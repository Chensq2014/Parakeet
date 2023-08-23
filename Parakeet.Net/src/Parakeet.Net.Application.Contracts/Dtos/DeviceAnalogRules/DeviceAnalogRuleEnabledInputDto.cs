using System;
using System.ComponentModel;
using Volo.Abp.Application.Dtos;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 规则启用禁用
    /// </summary>
    public class DeviceAnalogRuleEnabledInputDto : EntityDto<Guid>
    {
        /// <summary>
        /// 是否启用规则  默认为false禁用状态
        /// </summary>
        [Description("是否启用状态  默认为false")]
        public bool IsEnabled { get; set; }
    }
}
