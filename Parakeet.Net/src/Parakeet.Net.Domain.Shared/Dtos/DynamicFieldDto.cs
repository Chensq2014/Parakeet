using Parakeet.Net.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 动态列(时间轴)
    /// </summary>
    [Description("动态列基类")]
    public class DynamicFieldDto : DynamicFieldDto<decimal>
    {

    }
    /// <summary>
    /// 动态列泛型基类
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    [Description("动态列泛型基类")]
    public class DynamicFieldDto<T> : BaseDto
    {
        /// <summary>
        /// 属性/字段名称
        /// </summary>
        [Description("属性/字段名称")]
        public string DataField { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        [Description("别名")]
        public string Caption { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        [Description("日期")]
        public DateTime? Date { get; set; }

        /// <summary>
        /// 是否允许编辑
        /// </summary>
        [Description("是否允许编辑")]
        public bool AllowEditing { get; set; }

        /// <summary>
        /// 提供给用户自定义
        /// </summary>
        [Description("允许自定义")]
        public bool Custom { get; set; }

        /// <summary>
        /// 季度
        /// </summary>
        [Description("季度")]
        public int? Season { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [Description("值")]
        public T Value { get; set; }

        /// <summary>
        /// 上限
        /// </summary>
        [Description("上限")]
        public T Limit { get; set; }

        /// <summary>
        /// 子类动态列
        /// </summary>
        [Description("子类动态列"), NotSet]
        public List<DynamicFieldDto<T>> Children { get; set; } = new List<DynamicFieldDto<T>>();
    }
}
