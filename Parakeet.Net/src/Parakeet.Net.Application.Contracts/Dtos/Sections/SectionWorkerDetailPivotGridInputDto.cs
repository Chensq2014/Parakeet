using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Parakeet.Net.Enums;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// PivotGridInputDto
    /// </summary>
    public class SectionWorkerDetailPivotGridInputDto: InputDateTimeDto
    {
        
        /// <summary>
        /// 工作位置名称
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255)]
        [Description("工作位置名称")]
        public string PositionName { get; set; }

        /// <summary>
        /// 区域工人
        /// </summary>
        [Description("区域工人")]
        public Guid? SectionWorkerId { get; set; }
    }
}
