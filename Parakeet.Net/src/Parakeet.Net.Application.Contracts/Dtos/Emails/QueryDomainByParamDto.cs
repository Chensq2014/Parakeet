namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 根据参数查询域名
    /// </summary>
    public class QueryDomainByParamDto
    {
        //密码
        public string KeyWord { get; set; }

        /// <summary>
        /// 0 表示正常
        /// 1 表示不正常
        /// </summary>
        public int? Status { get; set; }
    }
}
