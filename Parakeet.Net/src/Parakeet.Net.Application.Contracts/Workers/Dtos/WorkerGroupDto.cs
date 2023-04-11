namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 人员班组信息
    /// </summary>
    public class WorkerGroupDto : BaseDto
    {
        /// <summary>
        /// 是否主分配（true：是,false:否）
        /// </summary>
        public bool IsMain { get; set; }
    }
}
