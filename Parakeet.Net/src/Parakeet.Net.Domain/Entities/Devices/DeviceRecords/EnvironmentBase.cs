using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 环境监测
    /// </summary>
    [Table("Parakeet_EnvironmentRecords", Schema = "parakeet")]
    public class EnvironmentBase : DeviceRecordBase
    {
        public EnvironmentBase()
        {
        }

        public EnvironmentBase(Guid id) : base(id)
        {
        }

        /// <summary>
        /// PM2.5
        /// </summary>
        [Description("PM2.5")]
        public decimal? PM2P5 { get; set; }

        /// <summary>
        /// PM10
        /// </summary>
        [Description("PM10")]
        public decimal? PM10 { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        [Description("温度")]
        public decimal? Temperature { get; set; }

        /// <summary>
        /// 噪声
        /// </summary>
        [Description("噪声")]
        public decimal? Noise { get; set; }

        /// <summary>
        /// 湿度
        /// </summary>
        [Description("湿度")]
        public decimal? Humidity { get; set; }

        /// <summary>
        /// 风向
        /// </summary>
        [Description("风向")]
        public decimal? WindDirection { get; set; }

        /// <summary>
        /// 风速
        /// </summary>
        [Description("风速")]
        public decimal? WindSpeed { get; set; }

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
        /// 一氧化碳
        /// </summary>
        [Description("一氧化碳")]
        public decimal? CO { get; set; }

        /// <summary>
        /// 二氧化硫
        /// </summary>
        [Description("二氧化硫")]
        public decimal? SO2 { get; set; }

        /// <summary>
        /// 二氧化氮
        /// </summary>
        [Description("二氧化氮")]
        public decimal? NO2 { get; set; }

        /// <summary>
        /// 臭氧
        /// </summary>
        [Description("臭氧")]
        public decimal? O3 { get; set; }

        /// <summary>
        /// 能见度
        /// </summary>
        [Description("能见度")]
        public decimal? Visibility { get; set; }

        /// <summary>
        /// 总挥发性有机化合物
        /// 苯类、烷类、芳烃类、烯类、卤烃类、酯类、醛类、酮类和其他。
        /// </summary>
        [Description("总挥发性有机化合物(苯类、烷类、芳烃类、烯类、卤烃类、酯类、醛类、酮类和其他)")]
        public decimal? TVOC { get; set; }

        /// <summary>
        /// 空气质量指数
        /// </summary>
        [Description("空气质量指数")]
        public decimal? AQI { get; set; }

        /// <summary>
        /// 总悬浮颗粒物
        /// </summary>
        [Description("总悬浮颗粒物")]
        public decimal? TSP { get; set; }
    }
}
