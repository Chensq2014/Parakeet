using Parakeet.Net.CustomAttributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 检查手机号
    /// </summary>
    public class CheckPhoneInput : IValidatableObject
    {
        /// <summary>
        /// 电话号码
        /// </summary>
        [Required, Regex(Regexes.TelePhone)]//Regex(@"^1[3456789]\d{9}$"
        public string Phone { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 是否对手机号进行重复性检查
        /// </summary>
        public bool IsCheckPhone { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Phone.Validate())
            {
                yield return new ValidationResult("请输入正确的手机号码!");
            }
        }
    }
}
