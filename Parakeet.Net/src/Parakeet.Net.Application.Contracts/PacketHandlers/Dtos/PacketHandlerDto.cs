using Parakeet.Net.Dtos;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// tcp包头 传输对象
    /// </summary>
    public class PacketHandlerDto : BaseDto
    {
        /// <summary>
        /// 包头
        /// </summary>
        [Required, Description("包头"), MaxLength(CustomerConsts.MaxLength255)]
        public string Header { get; set; }

        /// <summary>
        /// 处理器
        /// </summary>
        [Required, Description("处理器"), MaxLength(CustomerConsts.MaxLength255)]
        public string Handler { get; set; }

        public override string ToString()
        {
            return $"{Header}:{Handler}";
        }
    }
}
