using System;
using System.Collections.Generic;
using System.Text;

namespace Parakeet.Net.Dtos.Requests
{
    /// <summary>
    /// 企业微信获取部门列表返回对象
    /// </summary>
    public class WeixinDepartmentListResponseDto: WebClientResultBase
    {
        /// <summary>
        /// 部门列表数据
        /// </summary>
        public List<WeixinDepartment> Department_id { get; set; } = new List<WeixinDepartment>();
    }

    public class WeixinDepartment
    {
        /// <summary>
        /// 创建的部门id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 父部门id。根部门为1。
        /// </summary>
        public int Parentid { get; set; }
        /// <summary>
        /// 在父部门中的次序值。order值大的排序靠前。值范围是[0, 2^32)。
        /// </summary>
        public int Order { get; set; }
    }
}
