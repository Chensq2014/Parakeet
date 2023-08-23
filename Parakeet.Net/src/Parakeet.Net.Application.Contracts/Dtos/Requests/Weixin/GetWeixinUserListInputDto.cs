using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 获取企业微信用户id列表输入类
    /// </summary>
    public class GetWeixinUserListInputDto
    {
        /// <summary>
        /// cursor 分页信息 上一次请求返回
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// limit 预期请求的数据量 默认10000
        /// </summary>
        public long Limit { get; set; } = 10000;
    }
}
