using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Parakeet.Net.Dtos
{
    public class NotifyDeleteAllInput : NotifyInput, IValidatableObject
    {
        public NotifyDeleteAllInput()
        {
            SelectedIds = new List<Guid>();
        }

        /// <summary>
        /// 选中消息Ids集合
        /// </summary>
        public List<Guid> SelectedIds { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!SelectedIds.Any())
            {
                yield return new ValidationResult("至少选中一条记录!");
            }
        }
    }
}
