using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Parakeet.Net.CustomAttributes
{
    /// <summary>
    /// 正则表达式 Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field), Description("自定义正则表达式特性类")]
    public class RegexAttribute : AbstractValidateAttribute
    {
        /// <summary>
        /// 自定义正则表达式
        /// </summary>
        public string RegexExpression;

        /// <summary>
        /// 用户传入正则规则字符串：字符串格式参考EmailAttribute
        /// </summary>
        /// <param name="regex"></param>
        public RegexAttribute(string regex)
        {
            RegexExpression = regex;
        }
        /// <summary>
        /// 根据规则验证方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns>验证结果</returns>
        public override bool Validate<T>(T value)
        {
            return !string.IsNullOrWhiteSpace(value?.ToString()) && Regex.IsMatch(value.ToString(), RegexExpression, RegexOptions.IgnoreCase);
        }
    }
}
