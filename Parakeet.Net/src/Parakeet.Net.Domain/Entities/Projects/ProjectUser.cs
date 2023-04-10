using System;
using Parakeet.Net.CustomAttributes;
using Parakeet.Net.Users;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parakeet.Net.Entities.Projects
{
    /// <summary>
    /// 项目用户
    /// </summary>
    [Description("项目用户")]
    public class ProjectUser : BaseEntity
    {
        public ProjectUser()
        {
        }
        public ProjectUser(Guid id)
        {
            SetEntityPrimaryKey(id);
        }
        #region 项目

        /// <summary>
        /// 项目 ProjectId
        /// </summary>
        [Description("项目")]
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        [NotSet, Description("项目")]
        public virtual Project Project { get; set; }

        #endregion

        #region 项目用户

        /// <summary>
        /// 用户Id
        /// </summary>
        [Description("用户")]
        public Guid? UserId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [NotSet, Description("用户")]
        public virtual AppUser User { get; set; }

        #endregion
    }
}
