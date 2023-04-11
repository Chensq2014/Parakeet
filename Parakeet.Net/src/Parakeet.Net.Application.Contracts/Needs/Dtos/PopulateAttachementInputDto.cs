using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Needs.Dtos
{
    public class PopulateAttachementInputDto : IValidatableObject
    {
        /// <summary>
        /// 需求Id
        /// </summary>
        [Required, Description("需求Id")]
        public Guid? NeedId { get; set; }

        /// <summary>
        /// 是否允许删除
        /// </summary>
        [Description("是否允许删除")]
        public bool AllowDelete { get; set; }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!NeedId.HasValue)
            {
                yield return new ValidationResult("必须指定外键");
            }
        }
    }
}
