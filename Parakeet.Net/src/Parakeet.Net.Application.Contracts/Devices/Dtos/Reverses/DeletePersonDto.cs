namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 从设备删除人员信息
    /// </summary>
    public class DeletePersonDto : EquipmentBaseDto
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public override string CommandName => "delete_person";

        /// <summary>
        /// 目前只支持每次删除一个人员，用人员id（personId）标识
        /// </summary>
        public string[] PersonnelIds { get; set; }
    }
}
