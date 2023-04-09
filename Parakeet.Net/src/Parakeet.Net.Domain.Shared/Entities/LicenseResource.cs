using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 许可证资源
    /// </summary>
    [Table("Parakeet_LicenseResources", Schema = "parakeet")]
    public class LicenseResource : BaseEntity
    {
        public LicenseResource() { }
        public LicenseResource(Guid id)
        {
            SetEntityPrimaryKey(id);
        }

        /// <summary>
        /// 资源名称
        /// </summary>
        [Description("资源名称"), MaxLength(CustomerConsts.MaxLength64)]
        public string Name { get; set; }

        /// <summary>
        /// 资源Code确保唯一性
        /// </summary>
        [Description("资源Code确保唯一性"), MaxLength(CustomerConsts.MaxLength64)]
        public string Code { get; set; }

        /// <summary>
        /// 资源类型 1=webApi
        /// </summary>
        [Description("资源类型 1=webApi")]
        public int ResourceType { get; set; }

        /// <summary>
        /// 如果类型为1=webApi，那么将匹配请求资源路由，支持正则表达式，如：^/api.*
        /// </summary>
        [Description("如果类型为1=webApi，那么将匹配请求资源路由，支持正则表达式，如：^/api.*"), MaxLength(CustomerConsts.MaxLength255)]
        public string Path { get; set; }

        /// <summary>
        /// 禁用
        /// </summary>
        [Description("禁用")]
        public bool Disabled { get; set; }

        /// <summary>
        /// LicenseId
        /// </summary>
        [Description("LicenseId")]
        public Guid LicenseId { get; set; }

        /// <summary>
        /// License
        /// </summary>
        public virtual License License { get; set; }
    }
}
