//using System;
//using System.Collections.Generic;
//using System.ComponentModel;

//namespace Parakeet.Net.Entities
//{
//    /// <summary>
//    /// 许可证
//    /// </summary>
//    //[Table("Parakeet_Licenses", Schema = "parakeet")]
//    public class License : BaseEntity
//    {
//        public License()
//        {
//        }

//        public License(Guid id)
//        {
//            base.SetEntityPrimaryKey(id);
//        }

//        /// <summary>
//        /// AppId
//        /// </summary>
//        [Description("AppId")]
//        public string AppId { get; set; }

//        /// <summary>
//        /// AppKey
//        /// </summary>
//        [Description("AppKey")]
//        public string AppKey { get; set; }

//        /// <summary>
//        /// AppId
//        /// </summary>
//        [Description("AppId")]
//        public string AppSecret { get; set; }

//        /// <summary>
//        /// Token
//        /// </summary>
//        [Description("Token")]
//        public string Token { get; set; }

//        /// <summary>
//        /// 过期时间
//        /// </summary>
//        [Description("过期时间")]
//        public DateTime ExpiredAt { get; set; }

//        /// <summary>
//        /// 名称
//        /// </summary>
//        [Description("名称")]
//        public string Name { get; set; }

//        /// <summary>
//        /// 可访问资源
//        /// </summary>
//        public virtual ICollection<LicenseResource> LicenseResources { get; set; } = new HashSet<LicenseResource>();
//    }
//}