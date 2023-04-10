using Parakeet.Net.Entities.Houses;
using Parakeet.Net.Entities.LocationAreas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parakeet.Net.Entities.Sections
{
    /// <summary>
    /// 小区
    /// </summary>
    [Table("Parakeet_Sections", Schema = "public")]
    [Description("小区")]
    public class Section : BaseEntity
    {
        public Section()
        {
        }

        public Section(Guid id)
        {
            SetEntityPrimaryKey(id);
        }

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
        public virtual LocationArea LocationArea { get; set; }

        #endregion

        #region 小区住户

        /// <summary>
        /// 小区住户房间
        /// </summary>
        public virtual ICollection<House> Houses { get; set; } = new HashSet<House>();

        #endregion

    }
}
