using System;
using Volo.Abp.Application.Dtos;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    ///     可空Guid/DateTime类型的输入基类
    /// </summary>
    public class InputDateTimeDto : IEntityDto<Guid?>
    {
        ///// <summary>
        /////     可空主/外键Guid类型 Id必填
        ///// </summary>
        //[Required]
        public Guid? Id { get; set; }

        ///// <summary>
        /////     开始时间
        ///// </summary>
        //[Required]
        public DateTime? StartDate { get; set; }

        ///// <summary>
        /////     结束时间
        ///// </summary>
        //[Required]
        public DateTime? EndDate { get; set; }
    }
}