namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 下发人员基本信息对象
    /// </summary>
    public class UfacePersonCreateDto
    {
        /// <summary>
        /// interfaceName
        /// </summary>
        public string InterfaceName { get; set; }

        /// <summary>
        /// 任务编号
        /// </summary>
        public string TaskNo { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 人员信息
        /// </summary>
        public UfacePersonDto Person { get; set; }
    }
}