using System;
using System.ComponentModel;
using Volo.Abp.Domain.Entities.Auditing;

namespace ConsoleApp.Dtos.XiamenHuizhan
{
    /// <summary>
    /// 环境(多个标段综合字段)
    /// </summary>
    public class EnvironmentCommon : FullAuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// 环境
        /// </summary>
        public EnvironmentCommon() { }

        /// <summary>
        /// 环境
        /// </summary>
        /// <param name="id"></param>
        public EnvironmentCommon(Guid id) : base(id)
        {

        }

        /// <summary>
        /// 设备编码（设备真实设备序列号）
        /// </summary>
        [Description("设备编码")]
        public string SerialNo { get; set; }

        /// <summary>
        /// 转发编码（对外提供设备序列号）
        /// </summary>
        [Description("转发编码")]
        public string FakeNo { get; set; }

        /// <summary>
        /// PM25
        /// </summary>
        [Description("PM25")]
        public decimal? Pm25 { get; set; }

        /// <summary>
        /// PM25-Flag
        /// </summary>
        public string Pm25Flag { get; set; }

        /// <summary>
        /// PM10
        /// </summary>
        [Description("PM10")]
        public decimal? Pm10 { get; set; }

        /// <summary>
        /// PM10-Flag
        /// </summary>
        public string Pm10Flag { get; set; }

        /// <summary>
        /// 风速
        /// </summary>
        [Description("风速")]
        public decimal? WindSpeed { get; set; }

        /// <summary>
        /// 风速flag
        /// </summary>
        public string WindSpeedFlag { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        [Description("温度")]
        public decimal? Temperature { get; set; }

        /// <summary>
        /// TEM-Flag
        /// </summary>
        public string TemperatureFlag { get; set; }

        /// <summary>
        /// 噪声
        /// </summary>
        [Description("噪声")]
        public decimal? Noise { get; set; }

        /// <summary>
        /// B03-Flag
        /// </summary>
        public string NoiseFlag { get; set; }

        /// <summary>
        /// 湿度
        /// </summary>
        [Description("湿度")]
        public decimal? Humidity { get; set; }

        /// <summary>
        /// RH-Flag
        /// </summary>
        public string HumidityFlag { get; set; }

        /// <summary>
        /// 风向
        /// </summary>
        [Description("风向")]
        public decimal? WindDirection { get; set; }

        /// <summary>
        /// WD-Flag
        /// </summary>
        public string WdFlag { get; set; }

        /// <summary>
        /// 雨量
        /// </summary>
        [Description("雨量")]
        public decimal? Rainfall { get; set; }

        /// <summary>
        /// 气压
        /// </summary>
        [Description("气压")]
        public decimal? Pressure { get; set; }

        /// <summary>
        /// PA-Flag
        /// </summary>
        public string PaFlag { get; set; }

        /// <summary>
        /// TSP-Rtd 总悬浮颗粒物
        /// </summary>
        public decimal? Tsp { get; set; }

        /// <summary>
        /// TSP-Flag
        /// </summary>
        public string TspFlag { get; set; }

        /// <summary>
        /// RecordTime
        /// </summary>
        public DateTime? RecordTime { get; set; }
    }
}
