using System.Collections.Generic;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 企业微信成员列表返回类
    /// </summary>
    public class WeixinUserListResponseDto: WebClientResultBase
    {

        /// <summary>
        /// 分页游标
        /// </summary>
        public string Next_cursor { get; set; }

        /// <summary>
        /// 部门用户列表
        /// </summary>
        public List<DeptUser> Dept_user { get; set; } = new List<DeptUser>();
    }
    public class DeptUser
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string Userid { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public int Department { get; set; }
    }
}
