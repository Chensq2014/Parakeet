using Parakeet.Net.Enums;
using Parakeet.Net.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    ///     省市区区域输入类
    /// </summary>
    public class LocationAreaInputDto : IValidatableObject
    {
        #region 基础信息字段

        /// <summary>
        ///     指定区域代码
        /// </summary>
        [Description("指定区域代码")]
        public string Code { get; set; }

        /// <summary>
        ///     父级区域代码
        /// </summary>
        [Description("父级区域代码")]
        public string ParentCode { get; set; }

        /// <summary>
        ///     父级区域Id，只需要传递这个参数即可
        /// </summary>
        [Description("父级区域")]
        public Guid? ParentId { get; set; }

        /// <summary>
        ///     区域深度 深度级别 0:国家(中国)；1:省份；2:市；3:区/县 4、居委会/街道 5、乡村
        /// </summary>
        [Description("区域深度")]
        public DeepLevelType? Level { get; set; }

        /// <summary>
        ///     取消验证参数
        /// </summary>
        [Description("取消验证参数")]
        public bool CancelValid { get; set; }

        #endregion
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!CancelValid && !(ParentId.HasValue || Level.HasValue || ParentCode.HasValue() || Code.HasValue()))
            {
                //throw new ArgumentException("请填写至少一个查询条件!");
                yield return new ValidationResult("请填写至少一个查询条件!");
            }
        }
    }
}