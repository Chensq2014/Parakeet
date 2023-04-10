using System;
using System.ComponentModel;

namespace Parakeet.Net.CustomAttributes
{
    /// <summary>
    /// 所有实体公共字段特性类(使用反射sql时有时需要排除这几个字段)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field), Description("所有实体公共字段特性类")]
    public class BaseFieldAttribute : Attribute
    {

    }
}
