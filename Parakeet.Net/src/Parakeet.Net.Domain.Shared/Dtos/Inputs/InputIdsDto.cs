using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    ///     基础输入类
    /// </summary>
    [Description("基础输入类")]
    public class InputIdsDto<TPrimaryKey> : IValidatableObject
    {
        /// <summary>
        ///     Ids必填
        /// </summary>
        //[Required]
        [Description("唯一标识")]
        public List<TPrimaryKey> Ids { get; set; } = new List<TPrimaryKey>();

        /// <summary>
        ///     取消验证参数
        /// </summary>
        [Description("取消验证参数")]
        public bool CancelValid { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!CancelValid && !Ids.Any())
            {
                yield return new ValidationResult("Ids集合不能为空");
            }
        }
    }

    /// <summary>
    ///     Guid类型的输入基类
    /// </summary>
    public class InputIdsDto : InputIdsDto<Guid>
    {
    }

    /// <summary>
    ///     Guid?类型的输入基类
    /// </summary>
    public class InputIdsNullDto : InputIdsDto<Guid?>
    {
    }
}