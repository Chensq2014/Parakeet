using System;
using System.ComponentModel;

namespace Parakeet.Net.CustomAttributes
{
    /// <summary>
    /// 邮件 Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field), Description("自定义邮件特性类")]
    public class EmailAttribute : AbstractValidateAttribute
    {
        public override bool Validate<T>(T value)
        {
            return Regexes.GetRegex(Regexes.Email).IsMatch(value?.ToString() ?? "");
            //return Regex.IsMatch(value.ToString(), Regxes.Email);
        }
    }
}
