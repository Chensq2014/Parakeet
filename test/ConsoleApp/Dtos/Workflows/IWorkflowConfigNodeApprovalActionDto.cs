using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Dtos
{
    public interface IWorkflowConfigNodeApprovalActionDto
    {
        /// <summary>
        /// Id信息
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        ///动作Code,ActionCode
        /// </summary>
        public string ActionCode { get; set; }
        
        /// <summary>
        /// 流程配置Id 整个流程共用
        /// </summary>
        public Guid? BasicInfoId { get; set; }

        /// <summary>
        /// 节点Id
        /// </summary>
        public Guid? NodeId { get; set; }

        /// <summary>
        /// 流程审批配置Id 用于保存流程节点审批配置 撤销、同意、拒绝、移交，加签按钮 每个节点审批均可自定义配置的流程动作按钮
        /// </summary>
        public Guid? ApprovalId { get; set; }
    }
}
