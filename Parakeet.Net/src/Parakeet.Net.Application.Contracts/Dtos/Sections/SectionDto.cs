using Parakeet.Net.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Parakeet.Net.Dtos;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 地块/小区
    /// </summary>
    [Description("地块/小区")]
    public class SectionDto : BaseDto
    {
        #region 基础字段

        /// <summary>
        /// 小区名称
        /// </summary>
        [Description("小区名称")]
        [MaxLength(CustomerConsts.MaxLength255)]
        public string Name { get; set; }

        /// <summary>
        /// 小区地址
        /// </summary>
        [Required]
        [Description("小区地址")]
        [MaxLength(CustomerConsts.MaxLength2048)]
        public string Address { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength1024)]
        [Description("描述")]
        public string Description { get; set; }


        #endregion

        #region 小区所在区域

        /// <summary>
        ///     区域Id
        /// </summary>
        [Description("区域Id")]
        public Guid? LocationAreaId { get; set; }

        /// <summary>
        ///     所在区域
        /// </summary>
        [Description("区域")]
        public virtual LocationAreaDto LocationArea { get; set; }

        #endregion

        #region 小区住户

        /// <summary>
        /// 小区住户房间
        /// </summary>
        public virtual ICollection<HouseDto> Houses { get; set; } = new HashSet<HouseDto>();

        #endregion

        #region 工区劳务人员

        /// <summary>
        /// 工区劳务人员
        /// </summary>
        public virtual List<SectionWorkerDto> SectionWorkers { get; set; } = new List<SectionWorkerDto>();

        #endregion

        #region 项目

        [Description("项目Id")]
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        public virtual ProjectDto Project { get; set; }

        #endregion

        #region 合计

        /// <summary>
        /// 合计
        /// </summary>
        [Description("合计")]
        public decimal? HouseTotal => Houses.Sum(m => m.Total ?? 0);

        /// <summary>
        /// 总工价
        /// </summary>
        [Description("总工价")]
        public decimal? CostTotal => SectionWorkers.Sum(m => m.CostTotal ?? 0);

        /// <summary>
        /// 总利润
        /// </summary>
        [Description("总利润")]
        public decimal? ProfitTotal => SectionWorkers.Sum(m => m.ProfitTotal ?? 0);

        /// <summary>
        /// 总计
        /// </summary>
        [Description("总计")]
        public decimal? WorkerTotal => SectionWorkers.Sum(m => m.Total ?? 0);

        #endregion

    }
}
