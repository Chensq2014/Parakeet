namespace ConsoleApp.Dtos.ShaquGuanwang
{
    /// <summary>
    /// 设备标定定位
    /// </summary>
    public class DeviceLocationDto
    {
        /// <summary>
        /// 标识器唯一编码
        /// </summary>
        public string Rfid { get; set; }

        /// <summary>
        /// 2000坐标系x轴
        /// </summary>
        public decimal? X { get; set; }

        /// <summary>
        /// 2000坐标系y轴
        /// </summary>
        public decimal? Y { get; set; }

        /// <summary>
        /// 高程
        /// </summary>
        public decimal? H { get; set; }
    }
}
