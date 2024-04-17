using System;

namespace Parakeet.Net.ROClient.Models
{
    public class VehicleBasic : ModelBase
    {
        public override string CommandName => "add_vehicle_basic";

        /// <summary>
        /// 基础信息Id
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 车辆类型
        /// 轿车:0
        /// 渣土车:1
        /// 罐车:2
        /// 运料车:3
        /// 混凝土搅拌车:4
        /// 其他:9
        /// </summary>
        public VehicleType Type { get; set; }

        /// <summary>
        /// 车牌
        /// </summary>
        public string CarNumber { get; set; }

        /// <summary>
        /// 驾驶员Id
        /// </summary>
        public string DriverId { get; set; }

        /// <summary>
        /// 驾驶员姓名
        /// </summary>
        public string DriverName { get; set; }

        /// <summary>
        /// 驾驶证
        /// </summary>
        public string DriverLicense { get; set; }

        /// <summary>
        /// 到期时间(yyyy-MM-dd)
        /// </summary>
        public DateTime? ExpirationTime { get; set; }

        /// <summary>
        /// 驾驶到期时间
        /// </summary>
        public DateTime? DriveEndTime { get; set; }

        /// <summary>
        /// 驾驶员性别
        /// 男:1
        /// 女:2
        /// </summary>
        public GenderType Gender { get; set; } = GenderType.Male;

        /// <summary>
        /// 出生日期(yyyy-MM-dd)
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}
