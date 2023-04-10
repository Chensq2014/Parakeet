using System;
using System.ComponentModel;

namespace Parakeet.Net.CustomAttributes
{
    /// <summary>
    /// NotConnectAttribute  用一个特性来表示不需要数据库读取的属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field), Description("不需要sql读取赋值特性类")]
    public class NotSetAttribute : Attribute
    {

    }
}
