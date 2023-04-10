using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    ///     基础输入Url类
    /// </summary>
    [Description("基础输入Url类")]
    public class InputUrlDto
    {
        /// <summary>
        ///     Url必填
        /// </summary>
        [Required]
        [Description("Url")]
        public string Url { get; set; }
    }
}