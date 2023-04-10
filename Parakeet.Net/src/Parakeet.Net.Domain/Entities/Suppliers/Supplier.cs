using Parakeet.Net.Entities.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Parakeet.Net.Entities.LocationAreas;
using Parakeet.Net.ValueObjects;

namespace Parakeet.Net.Entities.Suppliers
{
    /// <summary>
    /// 供应商
    /// </summary>
    [Description("供应商")]
    [Table("Parakeet_Suppliers", Schema = "parakeet")]
    public class Supplier : BaseEntity
    {
        public Supplier()
        {

        }
        public Supplier(Guid id)
        {
            SetEntityPrimaryKey(id);
        }

        #region 基础字段

        /// <summary>
        /// 供应商名称
        /// </summary>
        [Required]
        [MaxLength(CustomerConsts.MaxLength255)]
        [Description("供应商名称")]
        public string Name { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [Required]
        [MaxLength(CustomerConsts.MaxLength16)]
        [Description("编码")]
        public string Code { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength1024)]
        [Description("描述")]
        public string Description { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength16)]
        [Description("联系方式")]
        public string Phone { get; set; }

        #endregion

        #region 地址/位置信息

        /// <summary>
        /// 地址
        /// </summary>
        public virtual Address Address { get; set; }

        /// <summary>
        ///     区域Id
        /// </summary>
        [Description("区域Id")]
        public Guid? LocationAreaId { get; set; }

        /// <summary>
        ///     所在区域
        /// </summary>
        [Description("区域")]
        public virtual LocationArea LocationArea { get; set; }

        #endregion

        #region 设备

        /// <summary>
        /// 设备
        /// </summary>
        public virtual ICollection<Device> Devices { get; set; } = new HashSet<Device>();

        #endregion
    }
}
