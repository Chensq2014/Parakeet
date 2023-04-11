using Parakeet.Net.CustomAttributes;
using Parakeet.Net.Dtos;
using Parakeet.Net.Enums;
using Parakeet.Net.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Needs.Dtos
{
    public class NeedDto : BaseDto//, IValidatableObject
    {
        #region 基础字段

        /// <summary>
        /// 客户名称
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255), Description("客户名称"), Required]
        public string Name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Description("性别")]
        public Sex? Sex { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength32), Description("手机")]
        public string Phone { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength32), Description("QQ")]
        public string QNumber { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength128), Description("邮箱")]
        public string Email { get; set; }

        /// <summary>
        /// 需求明细
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength2048), Description("需求明细"), Required]
        public string Requirements { get; set; }

        /// <summary>
        /// 是否阅读邮件
        /// </summary>
        [Description("是否阅读邮件")]
        public bool IsRead { get; set; }

        /// <summary>
        /// 阅读邮件时间
        /// </summary>
        [Description("阅读邮件时间")]
        public DateTime? ReadTime { get; set; }

        #endregion

        #region 地址

        /// <summary>
        /// 地址
        /// </summary>
        [Description("地址"), NotSet]
        public virtual Address Address { get; set; } = new Address();

        #endregion

        #region 附件

        /// <summary>
        /// 附件
        /// </summary>
        [Description("附件"), NotSet]
        public virtual List<NeedAttachmentDto> Attachments { get; set; } = new List<NeedAttachmentDto>();

        #endregion
    }
}
