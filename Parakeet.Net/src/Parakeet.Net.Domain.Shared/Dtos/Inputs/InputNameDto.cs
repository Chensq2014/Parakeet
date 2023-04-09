using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    ///     基础输入类
    /// </summary>
    [Description("基础输入类")]
    public class InputNameDto
    {
        /// <summary>
        ///     名称必填
        /// </summary>
        [Required]
        [Description("名称")]
        public string Name { get; set; }
    }
}