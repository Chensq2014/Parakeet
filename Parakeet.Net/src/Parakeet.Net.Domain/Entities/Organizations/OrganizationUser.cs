using Parakeet.Net.CustomAttributes;
using Parakeet.Net.Users;
using System;
using System.ComponentModel;
using Volo.Abp.Identity;

namespace Parakeet.Net.Entities.Organizations
{
    [Description("岗位用户")]
    public class OrganizationUser : BaseEntity
    {
        public OrganizationUser()
        {
        }
        public OrganizationUser(Guid id)
        {
            SetEntityPrimaryKey(id);
        }

        #region 机构

        /// <summary>
        /// 机构(类型为岗位) OrganizationId
        /// </summary>
        [Description("岗位")]
        public Guid? OrganizationId { get; set; }

        /// <summary>
        /// 机构(类型为岗位)
        /// </summary>
        [NotSet, Description("机构(类型为岗位)")]
        public virtual Organization Organization { get; set; }

        #endregion

        #region 用户

        /// <summary>
        /// 用户
        /// </summary>
        [Description("用户")]
        public Guid? UserId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [NotSet, Description("用户")]
        public virtual IdentityUser User { get; set; }

        #endregion
    }
}
