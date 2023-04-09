using System;
using Volo.Abp.Application.Dtos;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    ///     Dto基类
    /// </summary>
    //[Serializable, Description("Dto基类")]
    public abstract class BaseDto : BaseDto<Guid>
    {
    }

    //[Serializable, Description("Dto泛型基类")]//EntityDto AuditedEntityDto FullAuditedEntityDto
    public abstract class BaseDto<TPrimaryKey> : EntityDto<TPrimaryKey>//一般只需要使用EntityDto即可
    {
        #region Dto基本字段

        ///// <summary>
        /////     创建时间
        ///// </summary>
        //[Description("创建时间")]
        //public DateTime CreationTime { get; set; }

        ///// <summary>
        /////     最近一次更新时间
        ///// </summary>
        //[Description("更新时间")]
        //public DateTime? LastModificationTime { get; set; }

        ///// <summary>
        /////     删除时间
        ///// </summary>
        //[Description("删除时间")]
        //public DateTime? DeletionTime { get; set; }

        ///// <summary>
        /////     创建人（外键）
        ///// </summary>
        //[Description("创建人")]
        //public Guid? CreatorId { get; set; }

        ///// <summary>
        /////     修改人（外键）
        ///// </summary>
        //[Description("修改人")]
        //public Guid? LastModifierId { get; set; }

        ///// <summary>
        /////     删除人
        ///// </summary>
        //[Description("删除人")]
        //public Guid? DeleterId { get; set; }

        //public bool IsDeleted { get; set; }


        ///// <summary>
        ///// 创建人
        ///// </summary>
        //[Description("创建人")]
        //public string CreatorName { get; set; }
        ///// <summary>
        ///// 修改人
        ///// </summary>
        //[Description("修改人")]
        //public string LastModifierName { get; set; }
        ///// <summary>
        ///// 租户
        ///// </summary>
        //[Description("租户")]
        //public Guid? TenantId { get; set; }

        #endregion
    }
}