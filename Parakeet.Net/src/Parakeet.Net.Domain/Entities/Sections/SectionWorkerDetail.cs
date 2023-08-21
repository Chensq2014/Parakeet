using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 地块(区域)工人 用工明细
    /// </summary>
    [Table("Parakeet_SectionWorkerDetails", Schema = "public")]
    [Description("地块(区域)工人")]
    public class SectionWorkerDetail : Entity<Guid>, IHasCreationTime,IMayHaveCreator
    {
        public SectionWorkerDetail()
        {
        }

        public SectionWorkerDetail(Guid id) : base(id)
        {
        }

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
        public virtual SectionWorker SectionWorker { get; set; }

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
    }
}
