namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 人员基本信息对象
    /// </summary>
    public class UfacePersonDto
    {
        /// <summary>
        /// 人员Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string IdNumber { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string IdCardNum { get; set; }
    }
}