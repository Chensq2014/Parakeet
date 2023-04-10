using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    ///     基础输入类
    /// </summary>
    [Description("基础输入类")]
    public class InputIdDto<TPrimaryKey> : IEntityDto<TPrimaryKey>
    {
        /// <summary>
        ///     Id必填
        /// </summary>
        [Required]
        [Description("唯一标识")]
        public TPrimaryKey Id { get; set; }
    }

    /// <summary>
    ///     Guid类型的输入基类
    /// </summary>
    public class InputIdDto : InputIdDto<Guid>
    {
    }

    /// <summary>
    ///     可空Guid类型的输入基类
    /// </summary>
    public class InputIdNullDto : IEntityDto<Guid?>
    {
        ///// <summary>
        /////     可空主/外键Guid类型 Id必填
        ///// </summary>
        //[Required]
        public Guid? Id { get; set; }
    }
}