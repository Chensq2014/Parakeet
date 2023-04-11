using Parakeet.Net.Dtos.Houses;
using Parakeet.Net.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos.Products
{
    /// <summary>
    /// 产品
    /// </summary>
    [Description("产品")]
    public class ProductDto : BaseDto
    {
        #region 基础字段

        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        [MaxLength(CustomerConsts.MaxLength255)]
        public string Name { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        [Description("单价")]
        public decimal? Price { set; get; }

        /// <summary>
        /// Amount
        /// </summary>
        [Description("数量")]
        public decimal? Amount { set; get; }

        /// <summary>
        /// 总价
        /// </summary>
        [Description("总价")]
        public decimal? Total => Price * Amount ?? 0;

        /// <summary>
        ///  收费类型
        /// </summary>
        [Description("收费类型")]
        public ChargeType ChargeType { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength1024)]
        [Description("描述")]
        public string Description { get; set; }

        #endregion

        #region 房间

        [Description("房间Id")]
        public Guid? HouseId { get; set; }

        /// <summary>
        /// 房间
        /// </summary>
        public virtual HouseDto House { get; set; }

        #endregion
    }
}
