using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 工种
    /// </summary>
    [Description("工种")]
    [Table("Parakeet_WorkerTypes", Schema = "parakeet")]
    public class WorkerType : BaseEntity
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
