namespace Parakeet.Net.Consumer.Standard.Dtos
{
    /// <summary>
    /// 四川省厅请求token返回错误
    /// </summary>
    public class ErrorDto
    {
        /// <summary>
        /// 错误代码 0—失败，1-成功
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 错误信息描述
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 内部代码
        /// </summary>
        public string Inner_code { get; set; }

    }
}
