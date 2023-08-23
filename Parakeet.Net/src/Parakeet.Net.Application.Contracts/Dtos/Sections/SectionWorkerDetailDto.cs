using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 地块(区域)工人 用工明细
    /// </summary>
    [Description("地块(区域)工人 用工明细")]
    public class SectionWorkerDetailDto : BaseDto
    {
        #region 用工明细信息

        /// <summary>
        /// 工作位置名称
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255)]
        [Description("工作位置名称")]
        public string PositionName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Description("开始时间")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Description("结束时间")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 数量/工时 Hour or Day
        /// </summary>
        [Description("数量/工时")]
        public decimal? Amount { set; get; }

        /// <summary>
        /// 单位工价 perHour or perDay
        /// </summary>
        [Description("人工单价")]
        public decimal? UnitPrice { set; get; }

        /// <summary>
        /// 单位利润
        /// </summary>
        [Description("单位利润")]
        public decimal? UnitProfit { set; get; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength1024)]
        [Description("描述")]
        public string Description { get; set; }

        #endregion

        #region 区域工人

        /// <summary>
        /// 区域工人
        /// </summary>
        [Description("区域工人")]
        public Guid? SectionWorkerId { get; set; }

        /// <summary>
        /// 区域工人
        /// </summary>
        public virtual SectionWorkerDto SectionWorker { get; set; }

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
        public decimal? CostTotal => UnitPrice * Amount ?? 0;

        /// <summary>
        /// 总利润
        /// </summary>
        [Description("总利润")]
        public decimal? ProfitTotal => UnitProfit * Amount ?? 0;

        /// <summary>
        /// 总计
        /// </summary>
        [Description("总计")]
        public decimal? Total => CostTotal ?? 0 + ProfitTotal ?? 0;

        #endregion
    }
}
