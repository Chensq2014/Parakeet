using Newtonsoft.Json;
using System.Collections.Generic;

namespace Parakeet.Net.ROClient.Models
{
    public class ProjectGeofenceAddedModel : ModelBase
    {
        public override string CommandName => "add_project_geofence";

        /// <summary>
        /// 名称(不能重复)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 编号(唯一)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public DeviceType DeviceType { get; set; }

        /// <summary>
        /// 围栏类型(1:圆形,2:多边形)
        /// 暂不支持圆形
        /// </summary>
        public int Type { get; set; } = 2;

        /// <summary>
        /// 围栏行为(1:禁止进入,0:禁止离开)
        /// </summary>
        public int Action { get; set; } = 1;

        /// <summary>
        /// 报警次数
        /// </summary>
        public int AlarmTimes { get; set; } = 3;

        /// <summary>
        /// 生效时段（精确到分钟）
        /// eg:"00:00-23:59"
        /// </summary>
        public string[] EffectivePeriodArray { get; set; } = new string[] { "00:00-23:59" };

        /// <summary>
        /// 地理坐标（暂不支持圆形）
        /// 圆：{x_point: 22.548981, y_point: 114.086654, r: "500"}
        /// 多边形：[{"x_point":22.552697,"y_point":114.081751},{"x_point":22.551172,"y_point":114.084219},{ "x_point":22.550042,"y_point":114.082223}]
        /// </summary>
        public List<Coord> CoordArray { get; set; } = new List<Coord>();
    }

    public class Coord
    {
        public Coord()
        {
        }

        /// <summary>
        /// 构造坐标
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        public Coord(double lng, double lat)
        {
            Latitude = lat;
            Longitude = lng;
        }

        /// <summary>
        /// 纬度
        /// </summary>
        [JsonProperty("y_point")]
        public double Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [JsonProperty("x_point")]
        public double Longitude { get; set; }
    }
}