//using System;
//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;

//namespace Parakeet.Net.Entities
//{
//    /// <summary>
//    /// 许可证资源
//    /// </summary>
//    //[Table("Parakeet_LicenseResources", Schema = "parakeet")]
//    public class LicenseResource : BaseEntity//Entity<Guid>
//    {
//        public LicenseResource()
//        {
//        }

//        public LicenseResource(Guid id)
//        {
//            base.SetEntityPrimaryKey(id);//Id = id;
//        }

//        /// <summary>
//        /// 资源名称
//        /// </summary>
//        [MaxLength(64), Description("资源名称")]
//        public string Name { get; set; }

//        /// <summary>
//        /// 资源Code确保唯一性
//        /// </summary>
//        [MaxLength(64), Description("资源Code确保唯一性")]
//        public string Code { get; set; }

//        /// <summary>
//        /// 资源类型 1=webApi
//        /// </summary>
//        [Description("资源类型 1=webApi")]
//        public int ResourceType { get; set; }

//        /// <summary>
//        /// 如果类型为1=webApi，那么将匹配请求资源路由，支持正则表达式，如：^/api.*
//        /// </summary>
//        [MaxLength(128), Description("如果类型为1=webApi，那么将匹配请求资源路由，支持正则表达式，如：^/api.*")]
//        public string Path { get; set; }

//        /// <summary>
//        /// 禁用
//        /// </summary>
//        [Description("禁用")]
//        public bool Disabled { get; set; }

//        /// <summary>
//        /// LicenseId
//        /// </summary>
//        [Description("LicenseId")]
//        public Guid LicenseId { get; set; }

//        /// <summary>
//        /// License
//        /// </summary>
//        public virtual License License { get; set; }
//    }
//}