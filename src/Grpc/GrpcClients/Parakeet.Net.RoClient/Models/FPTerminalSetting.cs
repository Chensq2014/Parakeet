namespace Parakeet.Net.ROClient.Models
{
    public class FPTerminalSetting : ModelBase
    {
        public override string CommandName => "terminal_setting";

        /// <summary>
        /// 必填项且是递增的
        /// </summary>
        public short Sequence { get; set; } = 1;

        /// <summary>
        /// 指令类型
        /// 0x01:IP设置
        /// 0x02:缓存周期
        /// 0x03:校时
        /// 0x04:数据上报频率
        /// 0X05:设备工作模式
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// IP设置
        /// </summary>
        /// <example>192.168.0.1: 0XC0 0XA8 0X00 0X01</example>
        public string IP { get; set; }

        /// <summary>
        /// 缓存周期（单位分钟）
        /// </summary>
        /// <example>360分钟: 0X01 0X68</example>
        public short? CacheCycle { get; set; }

        /// <summary>
        /// 校时(14字节ASCII码)
        /// </summary>
        /// <example>20200715102315</example>
        public string Time { get; set; }

        /// <summary>
        /// 数据上报频率（单位秒）
        /// </summary>
        /// <example>360s：0X01 0X68</example>
        public short? Frequency { get; set; }

        /// <summary>
        /// 设备工作模式
        /// </summary>
        /// <example>（模式一）0X01</example>
        public short? WorkingMode { get; set; }
    }
}