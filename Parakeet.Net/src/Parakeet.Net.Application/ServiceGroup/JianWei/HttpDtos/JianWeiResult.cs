using System.Text.Json.Serialization;

namespace Parakeet.Net.ServiceGroup.JianWei.HttpDtos
{
    public class JianWeiResult<T> where T : class
    {
        /// <summary>
        /// 状态代码0	执行成功1	参数格式错误2	数据错误3	未授权4	系统异常
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// 返回结果记录列表，数组格式
        /// </summary>
        [JsonPropertyName("content")]
        public T Content { get; set; }

        /// <summary>
        /// 分页信息
        /// </summary>
        public Page Page { get; set; }
    }

    public class Page
    {
        /// <summary>
        /// 每页记录数
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 当前页，首页number=0,第二页number=1,依次类推
        /// </summary>
        public int Number { get; set; }
    }
}
