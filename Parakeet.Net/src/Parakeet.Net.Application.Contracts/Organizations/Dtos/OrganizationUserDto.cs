using System;
using System.ComponentModel;
using Parakeet.Net.Dtos;

namespace Parakeet.Net.Organizations.Dtos
{
    /// <summary>
    /// 岗位用户
    /// </summary>
    [Description("岗位用户")]
    public class OrganizationUserDto : BaseDto
    {
        #region 机构

        /// <summary>
        /// 机构(类型为岗位) OrganizationId
        /// </summary>
        [Description("岗位")]
        public Guid? OrganizationId { get; set; }

        /// <summary>
        /// 机构(类型为岗位)
        /// </summary>
        [Description("机构(类型为岗位)")]
        public virtual OrganizationDto Organization { get; set; }

        #endregion

        #region 用户
        /// <summary>
        /// 用户
        /// </summary>
        [Description("用户")]
        public virtual UserDto User { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [Description("用户")]
        public Guid? UserId { get; set; }
        #endregion

    }
}
