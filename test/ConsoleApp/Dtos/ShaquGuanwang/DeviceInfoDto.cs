namespace ConsoleApp.Dtos.ShaquGuanwang
{
    /// <summary>
    /// 管网设备 接口返回
    /// </summary>
    public class DeviceInfoDto
    {
        /// <summary>
        /// 设备id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 设备Devid
        /// </summary>
        public string Devid { get; set; }

        /// <summary>
        /// 定位
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 设备组
        /// </summary>
        public string Devgroup { get; set; }

        /// <summary>
        /// 设备版本
        /// </summary>
        public string DevVersion { get; set; }

        /// <summary>
        /// 设备软件版本
        /// </summary>
        public string SoftVersion { get; set; }

        /// <summary>
        /// 电量
        /// </summary>
        public decimal? Energy { get; set; }

        /// <summary>
        /// 信号
        /// </summary>
        public long? RtuSignal { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        public decimal? Tem { get; set; }

        /// <summary>
        ///     经度
        /// </summary>
        public decimal? Lng { get; set; }

        /// <summary>
        ///     纬度
        /// </summary>
        public decimal? Lat { get; set; }

        /// <summary>
        ///     高程
        /// </summary>
        public decimal? Altitude { get; set; }


    }
}
