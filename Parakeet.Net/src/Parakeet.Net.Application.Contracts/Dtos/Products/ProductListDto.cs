using Parakeet.Net.Enums;
using Parakeet.Net.Extensions;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 产品列表
    /// </summary>
    [Description("产品")]
    public class ProductListDto : ProductDto
    {
        /// <summary>
        /// 收费项目
        /// </summary>
        [Description("收费项目")]
        [MaxLength(CustomerConsts.MaxLength255)]
        public string ChargeTypeDisplayName => ChargeType.DisplayName();

        /// <summary>
        /// 房间
        /// </summary>
        [Description("房间")]
        [MaxLength(CustomerConsts.MaxLength255)]
        public string HouseNumber { get; set; }

        /// <summary>
        /// 小区Id
        /// </summary>
        [Description("小区Id")]
        public Guid? SectionId { get; set; }

        /// <summary>
        /// 小区名称
        /// </summary>
        [Description("小区名称")]
        [MaxLength(CustomerConsts.MaxLength255)]
        public string SectionName { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreationTime { get; set; }
    }
}
