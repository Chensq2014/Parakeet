namespace Parakeet.Net.ROClient.Models
{
    public class PersonDeletedModel : ModelBase
    {
        public override string CommandName => "delete_person";

        /// <summary>
        /// 仅从设备移除
        /// </summary>
        public bool OnlyRemoveFromDevice { get; set; } = true;

        /// <summary>
        /// 目前只支持每次删除一个人员
        /// 用人员id（PersonId）标识[必填]
        /// </summary>
        /// <value></value>
        public string[] PersonnelIds { get; set; }
    }
}