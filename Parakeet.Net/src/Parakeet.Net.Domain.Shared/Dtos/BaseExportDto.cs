using System;
using System.ComponentModel;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 导出公共基类 excel里面数据默认为double类型
    /// </summary>
    [Description("导出公共基类")]
    public class BaseExportDto : BaseExportDto<double>
    {

    }

    /// <summary>
    /// 导出公共泛型基类
    /// </summary>
    /// <typeparam name="TValue">excel单元格cell的值数据类型，excel默认为double</typeparam>
    [Description("导出公共泛型基类")]
    public class BaseExportDto<TValue> where TValue : struct
    {
        /// <summary>
        /// 节点Id
        /// </summary>
        [Description("Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// ParentId
        /// </summary>
        [Description("父级Id")]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        [Description("层级")]
        public long? Level { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        public string Name { get; set; }

        /// <summary>
        /// 金额cell中的数值
        /// </summary>
        [Description("金额")]
        public TValue Amount { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        [Description("日期")]
        public DateTime? Date { get; set; }

    }
}
