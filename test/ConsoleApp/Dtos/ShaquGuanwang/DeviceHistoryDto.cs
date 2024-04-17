using System;

namespace ConsoleApp.Dtos.ShaquGuanwang
{
    /// <summary>
    /// 设备历史数据记录
    /// </summary>
    public class DeviceHistoryDto
    {
        /// <summary>
        /// 设备Devid
        /// </summary>
        public string Devid { get; set; }

        /// <summary>
        /// 采集时间
        /// </summary>
        public DateTime? CollectTime { get; set; }

        /// <summary>
        /// 是否报警 字符串
        /// </summary>
        public string IsAlarm { get; set; }

        /// <summary>
        /// 数据通道 字符串
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 设备类型 eg;井盖位移检测终端
        /// </summary>
        public string RtuName { get; set; }

        /// <summary>
        /// 通道 监测项目
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// 是否为自检通道 枚举
        /// </summary>
        public int? IsCheckSelf { get; set; }

    }
}
