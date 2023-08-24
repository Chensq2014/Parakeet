using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Parakeet.Net.Dtos
{
    public class AddProjectUserInputDto : BaseDto, IValidatableObject
    {
        /// <summary>
        /// 项目Id
        /// </summary>
        [Required]
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// 用户Ids
        /// </summary>
        public List<Guid> UserIds { get; set; } = new List<Guid>();

        #region Implementation of IValidatableObject

        /// <summary>
        /// 自定义基础数据验证
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!UserIds.Any())
            {
                yield return new ValidationResult($"请添加至少一个用户");
            }
        }

        #endregion
    }
}