using System.Collections.Generic;

namespace Parakeet.Net.Consumer.Chongqing.Dtos
{
    public class TokenResultDto
    {
        /// <summary>
        /// controls
        /// </summary>
        public List<string> Controls { get; set; } = new List<string>();

        /// <summary>
        /// token
        /// </summary>
        public CustomDto Custom { get; set; } = new CustomDto();

        /// <summary>
        /// 状态
        /// </summary>
        public StatusDto Status { get; set; } = new StatusDto();
    }
}
