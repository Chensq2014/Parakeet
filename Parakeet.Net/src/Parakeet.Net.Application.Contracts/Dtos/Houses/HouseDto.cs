using Parakeet.Net.Dtos.Products;
using Parakeet.Net.Dtos.Sections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Parakeet.Net.Dtos.Houses
{
    /// <summary>
    /// 住户
    /// </summary>
    [Description("住户")]
    public class HouseDto : BaseDto
    {
        #region 基础字段

        /// <summary>
        /// 房间号
        /// </summary>
        [Description("房间号")]
        [MaxLength(CustomerConsts.MaxLength255)]
        public string Number { get; set; }

        /// <summary>
        /// 建筑面积
        /// </summary>
        [Description("建筑面积")]
        public decimal? BuildingArea { set; get; }

        /// <summary>
        /// 使用面积
        /// </summary>
        [Description("使用面积")]
        public decimal? UseArea { set; get; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength1024)]
        [Description("描述")]
        public string Description { get; set; }


        #endregion

        #region 小区

        [Description("小区Id")]
        public Guid? SectionId { get; set; }

        /// <summary>
        /// 小区
        /// </summary>
        public virtual SectionDto Section { get; set; }

        #endregion

        #region 房间使用装修产品

        /// <summary>
        /// 房间使用装修产品
        /// </summary>
        public virtual ICollection<ProductDto> Products { get; set; } = new HashSet<ProductDto>();

        #endregion

        #region 合计字段

        /// <summary>
        /// 合计
        /// </summary>
        [Description("合计")]
        public decimal? Total => Products.Sum(m => m.Total ?? 0);

        #endregion

    }
}
