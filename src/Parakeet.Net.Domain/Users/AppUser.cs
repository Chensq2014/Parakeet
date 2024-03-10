using Common.ValueObjects;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Common;
using Common.CustomAttributes;
using Common.Enums;
using Volo.Abp.Identity;

namespace Parakeet.Net.Users
{
    /* This entity shares the same table/collection ("AbpUsers" by default) with the
     * IdentityUser entity of the Identity module.
     *
     * - You can define your custom properties into this class.
     * - You never create or delete this entity, because it is Identity module's job.
     * - You can query users from database with this entity.
     * - You can update values of your custom properties.
     */
    public class AppUser : IdentityUser
    {
        #region 扩展字段
        /* Add your own properties here. Example:
        *
        * public string MyProperty { get; set; }
        *
        * If you add a property and using the EF Core, remember these;
        *
        * 1. Update NetDbContext.OnModelCreating
        * to configure the mapping for your new property
        * 2. Update NetEfCoreEntityExtensionMappings to extend the IdentityUser entity
        * and add your new property to the migration.
        * 3. Use the Add-Migration to add a new database migration.
        * 4. Run the .DbMigrator project (or use the Update-Database command) to apply
        * schema change to the database.
        */

        /// <summary>
        /// 身份证号码
        /// </summary>
        [Regex(Regexes.IdCardNo)]
        [MaxLength(CommonConsts.MaxLength18)]
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
        public DateTime? BirthDay { get; set; }

        /// <summary>
        /// 签章的SignAccountId值
        /// </summary>
        [MaxLength(CommonConsts.MaxLength255)]
        public string SignAccountId { get; set; }

        /// <summary>
        /// 个人签章照片-Base64
        /// </summary>
        [MaxLength(CommonConsts.MaxLength8192)]
        public string Signature { get; set; }

        /// <summary>
        /// 个性签名
        /// </summary>
        [MaxLength(CommonConsts.MaxLength255)]
        public string Motto { get; set; }

        /// <summary>
        /// 头像图片Key或Base64String
        /// </summary>
        [Description("头像图片Key或Base64String")]
        [MaxLength(CommonConsts.MaxLength8192)]
        public string HeadPortraitKey { get; set; }

        /// <summary>
        /// 头像图片二进制
        /// </summary>
        [Description("头像图片二进制")]
        public byte[] HeadPortraitImage { get; set; }

        /// <summary>
        /// 是否完成实名认证
        /// </summary>
        [Description("是否完成实名认证")]
        public bool? IsRealName { get; set; }

        /// <summary>
        /// 是否完成新手引导
        /// </summary>
        [Description("是否完成新手引导")]
        public bool? IsCompleteGuide { get; set; }
        #endregion

        /// <summary>
        /// 默认构造函数
        /// </summary>
        private AppUser() : base()
        {
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="emailConfirmed"></param>
        /// <param name="phoneNumberConfirmed"></param>
        /// <param name="isActive"></param>
        /// <param name="tenantId"></param>
        public AppUser(string userName, string email = "",
            string name = "", string surname = "",
            string phoneNumber = "", bool emailConfirmed = false,
            bool phoneNumberConfirmed = false, bool isActive = false,
            Guid? tenantId = default) : base(Guid.NewGuid(), userName, email, tenantId)
        {
            Name = name;
            Surname = surname;
            EmailConfirmed = emailConfirmed;
            PhoneNumber = phoneNumber;
            PhoneNumberConfirmed = phoneNumberConfirmed;
            IsActive = isActive;
        }

        /// <summary>
        /// 添加地址或key-value值
        /// </summary>
        /// <param name="address"></param>
        public virtual void SetAddress(Address address)
        {
            ExtraProperties.Add(nameof(Address), address);
        }

        /// <summary>
        /// 更新姓名
        /// </summary>
        /// <param name="name"></param>
        public virtual void SetName(string name)
        {
            Name = name;
        }
    }
}
