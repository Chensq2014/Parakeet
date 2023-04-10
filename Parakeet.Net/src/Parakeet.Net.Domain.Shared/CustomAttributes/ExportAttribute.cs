using System;
using System.ComponentModel;

namespace Parakeet.Net.CustomAttributes
{
    /// <summary>
    /// 是否支持导出属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field), Description("是否支持导出属性")]
    public class ExportAttribute : Attribute
    {
    }
}
