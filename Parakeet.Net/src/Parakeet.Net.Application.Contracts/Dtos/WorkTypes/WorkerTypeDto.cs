using System.ComponentModel;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 工种
    /// </summary>
    public class WorkerTypeDto : BaseDto
    {
        /// <summary>
        /// 工种编码
        /// </summary>
        [Description("工种编码")]
        public string Code { get; set; }

        /// <summary>
        /// 工种名称
        /// </summary>
        [Description("工种名称")]
        public string Name { get; set; }
    }
}
