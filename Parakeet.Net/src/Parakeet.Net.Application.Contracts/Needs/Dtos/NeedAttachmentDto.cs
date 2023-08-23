using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Parakeet.Net.Dtos;

namespace Parakeet.Net.Dtos
{
    public class NeedAttachmentDto : BaseAttachmentDto, IValidatableObject
    {
        /// <summary>
        /// 需求外键
        /// </summary>
        [Description("需求外键")]
        public Guid? NeedId { get; set; }

        /// <summary>
        /// 需求信息
        /// </summary>
        [Description("需求")]
        public virtual NeedDto Need { get; set; }

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
