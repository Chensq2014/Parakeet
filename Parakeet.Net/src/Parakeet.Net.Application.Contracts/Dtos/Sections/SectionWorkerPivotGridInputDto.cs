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
    public class SectionWorkerPivotGridInputDto: InputDateTimeDto
    {
        /// <summary>
        /// 工种Id
        /// </summary>
        [Description("工种Id")]
        public Guid? WorkerTypeId { get; set; }

        /// <summary>
        /// 劳务人员Id
        /// </summary>
        [Description("劳务人员Id")]
        public Guid? WorkerId { get; set; }

        /// <summary>
        /// 工区名称
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255)]
        [Description("工区名称")]
        public string Name { get; set; }
    }
}
