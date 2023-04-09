using System;
using System.ComponentModel;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 前端编辑某个实体的一个字段，这个字段对应的属性名称Filed及属性值Value的选项类，
    /// 另包含实体的Id,判断是否相等Equal等信息，用于构建表达式目录树与已存数据判断是否重复
    /// </summary>
    [Description("实体属性/字段选项基类")]
    public class FieldCheckOptionDto : FieldCheckOptionDto<Guid?, string>
    {

    }

    /// <summary>
    /// 实体属性/字段选项泛型基类
    /// </summary>
    /// <typeparam name="TPrimaryKey">实体主键</typeparam>
    [Description("实体属性/字段选项泛型基类")]
    public class FieldCheckOptionDto<TPrimaryKey> : FieldCheckOptionDto<TPrimaryKey, string>
    {

    }

    /// <summary>
    /// 实体属性/字段选项泛型基类
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Description("实体属性/字段选项泛型基类")]
    public class FieldCheckOptionDto<TPrimaryKey, TValue>
    {
        public FieldCheckOptionDto()
        {
            Equal = true;
        }

        /// <summary>
        /// 实体属性/字段名称
        /// </summary>
        [Description("实体属性/字段名称")]
        public string Field { get; set; }

        /// <summary>
        /// 实体属性/字段 值
        /// </summary>
        [Description("实体属性/字段值")]
        public TValue Value { get; set; }

        /// <summary>
        /// 系统里面Entity主键为int Id
        /// </summary>
        [Description("实体主键")]
        public TPrimaryKey Id { get; set; }

        /// <summary>
        /// 检查条件:是否相等
        /// </summary>
        [Description("检查条件:是否相等")]
        public bool Equal { get; set; }

    }
}
