namespace Parakeet.Net.ROClient.Models
{
    public class PersonLocationBasicDeletedModel : ModelBase
    {
        public override string CommandName => "delete_personlocation_basic";

        /// <summary>
        /// 传感器Id
        /// </summary>
        public int SensorId { get; set; } = 1;
    }
}