using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 地块/小区
    /// </summary>
    [Table("Parakeet_Sections", Schema = "public")]
    [Description("地块")]
    public class Section : BaseEntity
    {
        public Section()
        {
        }

        public Section(Guid id)
        {
            base.SetEntityPrimaryKey(id);
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

        #region 小区

        [Description("项目Id")]
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// 小区
        /// </summary>
        public virtual Project Project { get; set; }

        #endregion
    }
}
