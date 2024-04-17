using System.Collections.Generic;

namespace ConsoleApp.Dtos
{
    public class WorkflowConfigNodeApprovalActionConfig : WorkflowConfigNodeApprovalActionDto
    {
        /// <summary>
        /// 是否支持移动端审批
        /// </summary>
        public bool IsMobileApprovable { get; set; }

        public List<PostParameterDto> Parameters { get; set; } = new List<PostParameterDto>();
    }
}
