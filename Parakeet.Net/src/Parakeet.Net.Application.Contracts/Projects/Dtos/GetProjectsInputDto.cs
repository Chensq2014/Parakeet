using Parakeet.Net.Dtos;
using System;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 获取项目分页
    /// </summary>
    public class GetProjectsInputDto : PagedInputDto
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? ToDate { get; set; }
    }
}