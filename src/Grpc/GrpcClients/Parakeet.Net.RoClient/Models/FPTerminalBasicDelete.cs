namespace Parakeet.Net.ROClient.Models
{
    public class FPTerminalBasicDelete : ModelBase
    {
        public override string CommandName => "terminal_basic_delete";

        /// <summary>
        /// 终端传感器编号
        /// </summary>
        public int? TerminalId { get; set; } = 1;

        /// <summary>
        /// 传感器类型
        /// </summary>
        public FPSensorType? Sensor { get; set; }

        /// <summary>
        /// 测点编号
        /// </summary>
        public short? PointNumber { get; set; }
    }
}