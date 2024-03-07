using Parakeet.Net.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 地块(区域)工人
    /// </summary>
    [Table("Parakeet_SectionWorkers", Schema = "public")]
    [Description("地块(区域)工人")]
    public class SectionWorker  : BaseEntity
    {
        public SectionWorker()
        {
        }

        public SectionWorker(Guid id)
        {
            base.SetEntityPrimaryKey(id);
        }
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
        public virtual Section Section { get; set; }

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
        public virtual WorkerType WorkType { get; set; }

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
        public virtual Worker Worker { get; set; }

        #endregion

        #region 劳务人员工作明细

        /// <summary>
        /// 劳务人员工作明细
        /// </summary>
        public virtual ICollection<SectionWorkerDetail> SectionWorkerDetails { get; set; } = new HashSet<SectionWorkerDetail>();

        #endregion
    }
}
