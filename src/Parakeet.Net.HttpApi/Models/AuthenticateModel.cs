using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Extensions;

namespace Parakeet.Net.Models
{
    public class AuthenticateModel : IValidatableObject
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required,StringLength(128)]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required,StringLength(128)]
        public string Password { get; set; }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!UserName.HasValue())
            {
                yield return new ValidationResult("用户名不能为空");
            }

            if (!Password.HasValue())
            {
                yield return new ValidationResult("密码不能为空!");
            }
        }

    }
}