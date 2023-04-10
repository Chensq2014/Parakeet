using Parakeet.Net.Entities.Products;
using Parakeet.Net.Entities.Sections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parakeet.Net.Entities.Houses
{
    /// <summary>
    /// 房间
    /// </summary>
    [Table("Parakeet_Houses", Schema = "public")]
    [Description("房间")]
    public class House : BaseEntity
    {
        public House()
        {
        }

        public House(Guid id)
        {
            SetEntityPrimaryKey(id);
        }

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
        public virtual Section Section { get; set; }

        #endregion

        #region 房间使用装修产品

        /// <summary>
        /// 房间使用装修产品
        /// </summary>
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();

        #endregion

    }
}
