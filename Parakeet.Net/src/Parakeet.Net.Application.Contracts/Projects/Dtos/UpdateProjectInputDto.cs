using System;

namespace Parakeet.Net.Projects.Dtos
{
    /// <summary>
    /// 更新项目
    /// </summary>
    public class UpdateProjectInputDto : ProjectDto
    {
        public Guid ProjectId { get; set; }

        /// <summary>
        /// 是否同步子单位工程的项目名称
        /// </summary>
        public bool SyncUnitProject { get; set; }
    }
}