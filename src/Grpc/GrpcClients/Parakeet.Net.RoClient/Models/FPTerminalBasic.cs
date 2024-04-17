namespace Parakeet.Net.ROClient.Models
{
    public class FPTerminalBasic : ModelBase
    {
        public override string CommandName => "terminal_basic";

        /// <summary>
        /// 终端传感器编号
        /// </summary>
        public int TerminalId { get; set; } = 1;

        /// <summary>
        /// 传感器类型
        /// </summary>
        public FPSensorType Sensor { get; set; }

        /// <summary>
        /// 测点编号
        /// </summary>
        public short PointNumber { get; set; } = 1;

        /// <summary>
        /// 基准点
        /// </summary>
        public bool BasePoint { get; set; } = false;

        /// <summary>
        /// 与基准点的距离
        /// </summary>
        public double? Distance { get; set; }

        /// <summary>
        /// 传感器长度
        /// </summary>
        public double? Length { get; set; }

        /// <summary>
        /// 传感器深度
        /// </summary>
        public double? Deep { get; set; }

        /// <summary>
        /// 量程(设备最大测量值)
        /// </summary>
        public double? Range { get; set; }

        /// <summary>
        /// 跨度
        /// </summary>
        public int? Span { get; set; } = 1;

        /// <summary>
        /// 系数
        /// </summary>
        public double? Coefficient { get; set; }

        /// <summary>
        /// 初始读数(多个因素逗号隔开)
        /// </summary>
        public string InitialValue { get; set; }
    }
}