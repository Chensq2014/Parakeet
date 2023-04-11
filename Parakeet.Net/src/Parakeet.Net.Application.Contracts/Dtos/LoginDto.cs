using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Parakeet.Net.Extensions;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 登录Dto
    /// </summary>
    public class LoginDto : IValidatableObject
    {
        /// <summary>
        /// 账号UserName/Email
        /// </summary>
        
        [Required, MaxLength(128)]
        public string UserNameOrEmailAddress { get; set; }

        /// <summary>
        /// 密码(根据情况选择是否md5加密，后端解密即可)
        /// </summary>
        [DataType(DataType.Password)]
        [Required, MinLength(6)]
        public string Password { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [Required]
        public string Verify { get; set; }

        /// <summary>
        /// 记住我
        /// </summary>
        public bool? RememberMe { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Password.HasValue() || Password.Length < 6)
            {
                yield return new ValidationResult("密码错误！");
            }
        }


        public Guid? TenanId { get; set; }
    }
}
