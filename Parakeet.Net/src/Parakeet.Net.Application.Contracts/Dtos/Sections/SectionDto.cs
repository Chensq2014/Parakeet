using Parakeet.Net.Dtos.Houses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Parakeet.Net.Dtos.Sections
{
    /// <summary>
    /// 小区
    /// </summary>
    [Description("小区")]
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

        #region 合计

        /// <summary>
        /// 合计
        /// </summary>
        [Description("合计")]
        public decimal? Total => Houses.Sum(m => m.Total ?? 0);

        #endregion

    }
}
