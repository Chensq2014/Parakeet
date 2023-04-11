using System;
using System.Collections.Generic;
using System.ComponentModel;
using Parakeet.Net.Enums;
using Volo.Abp.Application.Dtos;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    ///     省市区区域
    /// </summary>
    [Description("省市区区域")]
    public class LocationAreaDto : EntityDto<Guid>
    {
        #region 基础信息字段

        /// <summary>
        ///     父级区域
        /// </summary>
        [Description("父级区域")]
        public Guid? ParentId { get; set; }

        /// <summary>
        ///     父级区域
        /// </summary>
        [Description("父级区域")]
        public string ParentName { get; set; }

        /// <summary>
        ///     父级区域代码
        /// </summary>
        [Description("父级区域代码")]
        public string ParentCode { get; set; }

        /// <summary>
        ///     区域类型 深度级别
        /// </summary>
        [Description("区域类型")]
        public DeepLevelType Level { get; set; }

        /// <summary>
        ///     区域名称
        /// </summary>
        [Description("区域名称")]
        public string Name { get; set; }

        /// <summary>
        ///     英文缩写
        /// </summary>
        [Description("英文缩写")]
        public string ShortName { get; set; }

        /// <summary>
        ///     拼音首字母
        /// </summary>
        [Description("拼音首字母")]
        public string Prefix { get; set; }

        /// <summary>
        ///     拼音
        /// </summary>
        [Description("拼音")]
        public string Pinyin { get; set; }

        /// <summary>
        ///     区域代码
        /// </summary>
        [Description("区域代码")]
        public string Code { get; set; }

        /// <summary>
        ///     邮编
        /// </summary>
        [Description("邮编")]
        public string ZipCode { get; set; }

        /// <summary>
        ///     经度
        /// </summary>
        [Description("经度")]
        public decimal? Longitude { get; set; }

        /// <summary>
        ///     纬度
        /// </summary>
        [Description("纬度")]
        public decimal? Latitude { get; set; }

        #endregion

        #region 子区域

        /// <summary>
        /// 子区域
        /// </summary>
        public virtual List<LocationAreaDto> Chidren { get; set; } = new List<LocationAreaDto>();

        #endregion
    }
}