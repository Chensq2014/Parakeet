using Parakeet.Net.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 消息通知/列表
    /// </summary>
    [Description("消息通知")]
    public class Notify : BaseEntity
    {
        public Notify() { }

        public Notify(Guid id)
        {
            base.SetEntityPrimaryKey(id);
        }

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
        [ Description("源头数据类型")]
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

        #endregion

        #region 创建Notify并触发事件

        /// <summary>
        ///  设置公共Notify信息
        /// </summary>
        /// <param name="formUserId"></param>
        /// <param name="toUserId"></param>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <param name="sourceId"></param>
        /// <param name="targetId"></param>
        /// <param name="projectId"></param>
        /// <param name="organizationId"></param>
        /// <returns>Notify基础对象</returns>
        public static Notify SetBaseNotify(
            Guid? formUserId, Guid? toUserId,
            SourceOrTargetType? sourceType = null,
            SourceOrTargetType? targetType = null,
            Guid? sourceId = null, Guid? targetId = null,
            Guid? projectId = null, Guid? organizationId = null)
        {
            var notify = new Notify(Guid.NewGuid())
            {
                ReceiveTime = DateTime.Now,
                NotifyType = NotifyType.系统消息,//默认系统消息
                FromUserId = formUserId,
                ToUserId = toUserId,
                ProjectId = projectId,
                OrganizationId = organizationId,
                SourceType = sourceType,
                TargetType = targetType,
                SourceId = sourceId,
                TargetId = targetId
            };
            return notify;
        }

        #endregion
    }
}
