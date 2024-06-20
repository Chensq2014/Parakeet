namespace ConsoleApp.Dtos
{
    /// <summary>
    /// 奥斯恩(扬尘)设备请求结果对象
    /// </summary>
    public class AoSienRequestResultDto
    {
        /// <summary> 
        /// 系统返回码 1001 接口调用成功  1002 接口调用失败
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 系统返回消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 系统返回时间戳
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 是否在线
        /// </summary>
        public string Live { get; set; }

        /// <summary>
        /// 实时数据时间
        /// </summary>
        public string DateTime { get; set; }

        /// <summary>
        /// 监测因子
        /// </summary>
        public string Sensor { get; set; }

        /// <summary>
        /// 检测因子实时数据
        /// </summary>
        public decimal? Data { get; set; }

        /// <summary>
        /// 检测因子最大值数据
        /// </summary>
        public decimal? Max { get; set; }

        /// <summary>
        /// 检测因子最小值数据
        /// </summary>
        public decimal? Min { get; set; }

        /// <summary>
        /// 检测因子状态
        /// </summary>
        public string Flag { get; set; }

        /// <summary>
        /// 检测因子名称
        /// </summary>
        public string Name { get; set; }
    }
}
