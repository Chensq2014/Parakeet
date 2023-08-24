using Parakeet.Net.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 地块/小区 工人
    /// </summary>
    [Description("地块/小区 工人")]
    public class SectionWorkerDto : BaseDto
    {
        #region 基础字段

        /// <summary>
        /// 工区名称
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255)]
        [Description("工区名称")]
        public string Name { get; set; }

        /// <summary>
        /// 面积
        /// </summary>
        [Description("面积")]
        public decimal? CoverArea { set; get; }

        /// <summary>
        /// 是否临时
        /// </summary>
        [Description("是否临时")]
        public bool IsTemporary { set; get; }

        ///// <summary>
        ///// 开始时间
        ///// </summary>
        //[Description("开始时间")]
        //public DateTime? StartDate { get; set; }

        ///// <summary>
        ///// 结束时间
        ///// </summary>
        //[Description("结束时间")]
        //public DateTime? EndDate { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength1024)]
        [Description("描述")]
        public string Description { get; set; }


        #endregion

        #region 区域/地块

        /// <summary>
        /// 区域/地块Id
        /// </summary>
        [Description("区域/地块Id")]
        public Guid? SectionId { get; set; }

        /// <summary>
        /// 区域/地块
        /// </summary>
        public virtual SectionDto Section { get; set; }

        #endregion

        #region 工种

        /// <summary>
        /// 工种Id
        /// </summary>
        [Description("工种Id")]
        public Guid? WorkerTypeId { get; set; }

        /// <summary>
        /// 工种
        /// </summary>
        public virtual WorkerTypeDto WorkType { get; set; }

        /// <summary>
        /// 劳务类型
        /// </summary>
        public LaborType LaborType { get; set; }

        #endregion

        #region 劳务人员

        /// <summary>
        /// 劳务人员Id
        /// </summary>
        [Description("劳务人员Id")]
        public Guid? WorkerId { get; set; }

        /// <summary>
        /// 劳务人员
        /// </summary>
        public virtual WorkerDto Worker { get; set; }

        #endregion

        #region 劳务人员工作明细

        /// <summary>
        /// 劳务人员工作明细
        /// </summary>
        public virtual List<SectionWorkerDetailDto> SectionWorkerDetails { get; set; } = new List<SectionWorkerDetailDto>();

        #endregion

        #region IHasCreationTime,IMayHaveCreator

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid? CreatorId { get; set; }

        #endregion

        #region 合计

        /// <summary>
        /// 总工价
        /// </summary>
        [Description("总工价")]
        public decimal? CostTotal => SectionWorkerDetails.Sum(m => m.CostTotal ?? 0);

        /// <summary>
        /// 总利润
        /// </summary>
        [Description("总利润")]
        public decimal? ProfitTotal => SectionWorkerDetails.Sum(m => m.ProfitTotal ?? 0);

        /// <summary>
        /// 总计
        /// </summary>
        [Description("总计")]
        public decimal? Total => SectionWorkerDetails.Sum(m => m.Total ?? 0);
        
        #endregion
    }
}
