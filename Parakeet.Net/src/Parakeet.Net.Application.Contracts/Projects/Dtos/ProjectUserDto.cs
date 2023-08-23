using Parakeet.Net.Dtos;
using System;
using System.ComponentModel;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 项目/课程 用户/学员
    /// </summary>
    [Description("项目/课程 用户/学员")]
    public class ProjectUserDto : BaseDto
    {
        #region 项目

        /// <summary>
        /// 项目 ProjectId
        /// </summary>
        [Description("项目")]
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        [Description("项目")]
        public virtual ProjectDto Project { get; set; }

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
        [Description("用户")]
        public virtual UserDto User { get; set; }

        #endregion
    }
}