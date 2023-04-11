using Parakeet.Net.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Parakeet.Net.Interactive.Dtos
{
    /// <summary>
    /// 消息通知/列表
    /// </summary>
    [Description("消息通知")]
    public class NotifyDto : AuditedEntityDto<Guid>
    {

        #region 消息基本属性

        /// <summary>
        /// 消息标题 必填
        /// </summary>
        [Required, MaxLength(CustomerConsts.MaxLength128), Description("消息标题")]
        public string Title { get; set; }

        /// <summary>
        /// 消息明细(多设计一个字段装消息内容)
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength2048), Description("消息明细")]
        public string ContentDetail { get; set; }

        /// <summary>
        /// 接收时间
        /// </summary>
        [Description("接收时间")]
        public DateTime ReceiveTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 消息类型
        /// </summary>
        [Description("消息类型")]
        public NotifyType NotifyType { get; set; }

        /// <summary>
        /// 消息状态
        /// </summary>
        [Description("消息状态")]
        public bool IsRead { get; set; }

        #endregion

        #region 用户 企业 项目 等外键字段 可空

        /// <summary>
        /// 发送者 登陆用户
        /// </summary>
        [Description("发送者")]
        public Guid? FromUserId { get; set; }

        /// <summary>
        /// 接收者
        /// </summary>
        [Description("接收者")]
        public Guid? ToUserId { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        [Description("项目")]
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// 组织Id
        /// </summary>
        [Description("组织")]
        public Guid? OrganizationId { get; set; }

        /// <summary>
        /// (申请加入项目/公司  审批/驳回等)申请Id
        /// </summary>
        [Description("申请")]
        public Guid? ApplicationId { get; set; }

        /// <summary>
        /// 是否申请信息true(拒绝/审批:false)
        /// </summary>
        [Description("是否申请")]
        public bool IsRequest { get; set; }

        /// <summary>
        /// 连接状态 true(有连接/无连接或连接不可用:false)
        /// </summary>
        [Description("连接状态")]
        public bool LinkStatus { get; set; }

        /// <summary>
        /// 连接辅助消息(多设计一个字段装消息内容)
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength1024), Description("连接辅助消息")]
        public string LinkDetail { get; set; }

        /// <summary>
        /// 源头数据类型
        /// </summary>
        [Description("源头数据类型")]
        public SourceOrTargetType? SourceType { get; set; }

        /// <summary>
        /// 目标数据类型
        /// </summary>
        [Description("目标数据类型")]
        public SourceOrTargetType? TargetType { get; set; }

        /// <summary>
        /// 各种源头Id
        /// </summary>
        [Description("源头")]
        public Guid? SourceId { get; set; }

        /// <summary>
        /// 企业/公司/员工/申请 等一切目标指向的id
        /// </summary>
        [Description("目标")]
        public Guid? TargetId { get; set; }

        ///// <summary>
        ///// 发送者 登陆用户
        ///// </summary>
        //[Description("发送者")]
        //public string FromUserName { get; set; }

        ///// <summary>
        ///// 接收者
        ///// </summary>
        //[Description("接收者")]
        //public string ToUserName { get; set; }

        ///// <summary>
        ///// 项目
        ///// </summary>
        //[Description("项目")]
        //public string ProjectName  { get; set; }

        ///// <summary>
        ///// 源名称
        ///// </summary>
        //[Description("源名称")]
        //public string SourceName { get; set; }
        ///// <summary>
        ///// 目标名称
        ///// </summary>
        //[Description("目标名称")]
        //public string TargetName { get; set; }

        #endregion

    }
}
