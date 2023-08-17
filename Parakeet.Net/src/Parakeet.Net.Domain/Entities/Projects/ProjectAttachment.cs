using Parakeet.Net.CustomAttributes;
using System;
using System.ComponentModel;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 项目附件表
    /// </summary>
    [Description("项目附件表")]
    public class ProjectAttachment : BaseAttachmentEntity
    {
        public ProjectAttachment()
        {
        }

        public ProjectAttachment(Guid id)
        {
            base.SetEntityPrimaryKey(id);
        }

        #region 项目

        /// <summary>
        /// 项目外键
        /// </summary>
        [Description("项目外键")]
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// 项目实体
        /// </summary>
        [NotSet, Description("项目")]
        public virtual Project Project { get; set; }

        #endregion
    }
}
