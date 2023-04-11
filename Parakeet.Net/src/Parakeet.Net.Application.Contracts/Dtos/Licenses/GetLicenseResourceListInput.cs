using System;
using System.ComponentModel;
using Volo.Abp.Application.Dtos;

namespace Parakeet.Net.Dtos.Licenses
{
    public class GetLicenseResourceListInput : PagedInputDto
    {
        #region 基础过滤字段

        /// <summary>
        /// 资源名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 资源Code确保唯一性
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 资源类型 1=webApi
        /// </summary>
        public int? ResourceType { get; set; }

        /// <summary>
        /// 禁用
        /// </summary>
        [Description("禁用")]
        public bool? Disabled { get; set; }

        /// <summary>
        /// LicenseId
        /// </summary>
        public Guid? LicenseId { get; set; }

        #endregion

    }
}