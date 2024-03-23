using System.Collections.Generic;

namespace Parakeet.Net.ROClient.Models
{
    public class ProjectGeofenceDeviceAddedModel : ModelBase
    {
        public override string CommandName => "add_project_geofence_device";

        /// <summary>
        /// 传感器Id(默认1)
        /// </summary>
        public int SensorId { get; set; } = 1;

        /// <summary>
        /// 围栏编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public DeviceType DeviceType { get; set; }

        /// <summary>
        /// 人员列表
        /// </summary>
        public List<GeoFenceUser> Users { get; set; } = new List<GeoFenceUser>();

        /// <summary>
        /// 车辆列表
        /// </summary>
        public List<GeoFenceCar> Cars { get; set; } = new List<GeoFenceCar>();
    }

    public class GeoFenceUser
    {
        public GeoFenceUser()
        {
        }

        public GeoFenceUser(string name, string idCard)
        {
            Name = name;
            IdCard = idCard;
        }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
    }

    public class GeoFenceCar
    {
        public GeoFenceCar()
        {
        }

        public GeoFenceCar(string carNumber)
        {
            CarNumber = carNumber;
        }

        /// <summary>
        /// 车牌号码
        /// </summary>
        public string CarNumber { get; set; }
    }
}