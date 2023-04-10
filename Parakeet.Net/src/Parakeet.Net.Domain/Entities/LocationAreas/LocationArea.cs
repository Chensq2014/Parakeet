using Parakeet.Net.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parakeet.Net.Entities.LocationAreas
{
    /// <summary>
    ///     位置区域（全国省市区城乡等区域代码位置信息）
    /// </summary>
    [Description("全国省市区城乡等区域代码位置信息")]
    [Table("Parakeet_LocationAreas", Schema = "parakeet")]
    public class LocationArea : BaseEntity
    {
        public LocationArea()
        {
        }

        public LocationArea(Guid id)
        {
            SetEntityPrimaryKey(id);
        }

        #region 父级

        /// <summary>
        ///     父级区域Id
        /// </summary>
        [Description("父级区域Id")]
        public Guid? ParentId { get; set; }

        /// <summary>
        ///     父级区域
        /// </summary>
        [Description("父级区域")]
        public virtual LocationArea Parent { get; set; }

        #endregion

        #region 子级

        /// <summary>
        ///     子级区域
        /// </summary>
        [Description("子级区域")]
        public virtual HashSet<LocationArea> Children { get; set; } = new HashSet<LocationArea>();

        #endregion

        /// <summary>
        ///     区域代码
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength16),Description("区域代码")]
        public string Code { get; set; }

        /// <summary>
        ///     父级区域代码
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength16),Description("父级区域代码")]
        public string ParentCode { get; set; }

        /// <summary>
        ///     邮编
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength6),Description("邮编")]
        public string ZipCode { get; set; }

        /// <summary>
        ///     区域类型 区域深度
        /// </summary>
        [Description("区域深度")]
        public DeepLevelType Level { get; set; }

        /// <summary>
        ///     区域名称
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255),Description("区域名称")]
        public string Name { get; set; }

        /// <summary>
        ///     区域全名
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255),Description("区域全名")]
        public string FuallName { get; set; }

        /// <summary>
        ///     英文全名
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255),Description("英文全名")]
        public string InternationalName { get; set; }

        /// <summary>
        ///     英文缩写
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength128),Description("英文缩写")]
        public string ShortName { get; set; }

        /// <summary>
        ///     拼音
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength128),Description("拼音")]
        public string Pinyin { get; set; }

        /// <summary>
        ///     拼音首拼
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64),Description("拼音首拼")]
        public string Prefix { get; set; }

        #region 位置信息

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
    }
}