using System;
using System.ComponentModel;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 项目附件
    /// </summary>
    [Description("项目附件")]
    public class ProjectAttachmentDto : BaseAttachmentDto
    {
        #region 项目

        /// <summary>
        /// 项目ProjectId
        /// </summary>
        [Description("项目ProjectId")]
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        [Description("项目")]
        public virtual ProjectDto Project { get; set; }

        #endregion
    }
}