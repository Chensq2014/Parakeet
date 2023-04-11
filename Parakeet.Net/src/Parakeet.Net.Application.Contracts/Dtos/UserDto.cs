using Parakeet.Net.CustomAttributes;
using Parakeet.Net.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 用户
    /// </summary>
    [Description("用户")]
    public class UserDto : BaseDto
    {
        #region Base properties

        public virtual Guid? TenantId { get; set; }

        public virtual string UserName { get; set; }

        public virtual string Name { get; set; }

        public virtual string Surname { get; set; }

        public virtual string Email { get; set; }

        public virtual bool EmailConfirmed { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual bool PhoneNumberConfirmed { get; set; }

        #endregion

        #region 扩展字段
        /* Add your own properties here. Example:
         *
         * public virtual string MyProperty { get; set; }
         */

        /// <summary>
        /// 身份证号码
        /// </summary>
        [Regex(Regexes.IdCardNo)]
        [MaxLength(CustomerConsts.MaxLength18)]
        public string IdCardNo { get; set; }

        /// <summary>
        /// 用户状态(注意可空枚举类型的反射)
        /// </summary>
        [Description("用户状态")]
        public UserStatus? UserStatus { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        [Description("用户类型")]
        public UserType? UserType { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Description("性别")]
        public Sex? Sex { get; set; }

        /// <summary>
        /// 等级/可改为枚举
        /// </summary>
        [Description("等级")]
        public TreeNodeLevel? Level { get; set; }

        /// <summary>
        /// 最近登陆时间
        /// </summary>
        [Description("最近登陆时间")]
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [Description("生日")]
        public  DateTime? BirthDay { get; set; }

        /// <summary>
        /// 签章的SignAccountId值
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255)]
        public string SignAccountId { get; set; }

        /// <summary>
        /// 个人签章照片-Base64
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength8192)]
        public string Signature { get; set; }

        /// <summary>
        /// 个性签名
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255)]
        public string Motto { get; set; }

        /// <summary>
        /// 头像图片Key或Base64String
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength8192)]
        public string HeadPortraitKey { get; set; }

        /// <summary>
        /// 头像图片二进制
        /// </summary>
        public byte[] HeadPortraitImage { get; set; }

        /// <summary>
        /// 是否完成实名认证
        /// </summary>
        public bool? IsRealName { get; set; }

        /// <summary>
        /// 是否完成新手引导
        /// </summary>
        public bool? IsCompleteGuide { get; set; }
        #endregion
    }
}
