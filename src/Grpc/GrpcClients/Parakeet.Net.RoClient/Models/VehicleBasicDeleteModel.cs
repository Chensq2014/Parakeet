using System;

namespace Parakeet.Net.ROClient.Models
{
    public class VehicleBasicDeleteModel : ModelBase
    {
        public override string CommandName => "delete_vehicle_basic";

        /// <summary>
        /// 车辆基础信息Id
        /// </summary>
        public Guid Id { get; set; }
    }
}
