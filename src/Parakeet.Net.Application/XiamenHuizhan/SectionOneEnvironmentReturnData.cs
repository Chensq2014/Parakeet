using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Parakeet.Net.XiamenHuizhan
{
    /// <summary>
    /// 一标段返回数据外层对象 SectionOneEnvironmentReturnData
    /// </summary>
    public class SectionOneEnvironmentReturnData
    {
        /// <summary>
        /// 返回编码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 返回根节点数据
        /// </summary>
        public SectionOneEnvironmentRootData Data { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Msg { get; set; }
    }

    /// <summary>
    /// SectionOneEnvironmentRootData
    /// </summary>
    public class SectionOneEnvironmentRootData
    {
        /// <summary>
        /// 数据结构
        /// </summary>
        public SectionOneEnvironmentData Data { get; set; }

        /// <summary>
        /// 数据Id
        /// </summary>
        public long DataId { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 设备类型Id
        /// </summary>
        public string DeviceTypeId { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime? Time { get; set; }
    }

    /// <summary>
    /// SectionOneEnvironmentData
    /// </summary>
    public class SectionOneEnvironmentData
    {
        /// <summary>
        /// PM10
        /// </summary>
        [JsonProperty("PM10-Rtd")]
        public decimal? Pm10Rtd { get; set; }

        /// <summary>
        /// PM10-Flag
        /// </summary>
        [JsonProperty("PM10-Flag")]
        public string Pm10Flag { get; set; }

        /// <summary>
        /// PM25
        /// </summary>
        [JsonProperty("PM25-Rtd")]
        public decimal? Pm25Rtd { get; set; }

        /// <summary>
        /// PM25-Flag
        /// </summary>
        [JsonProperty("PM25-Flag")]
        public string Pm25Flag { get; set; }

        /// <summary>
        /// WS-Rtd 风速
        /// </summary>
        [JsonProperty("WS-Rtd")]
        public decimal? WsRtd { get; set; }

        /// <summary>
        /// WS-Flag
        /// </summary>
        [JsonProperty("WS-Flag")]
        public string WsFlag { get; set; }

        /// <summary>
        /// DataTime
        /// </summary>
        [JsonProperty("DataTime")]
        public string DataTime { get; set; }

        /// <summary>
        /// 格式化时间
        /// </summary>
        public DateTime? DateTimeFormat => DateTime.ParseExact(DataTime, "yyyyMMddHHmmss", CultureInfo.CurrentCulture);

        /// <summary>
        /// WdRtd 风向
        /// </summary>
        [JsonProperty("WD-Rtd")]
        public decimal? WdRtd { get; set; }

        /// <summary>
        /// WD-Flag
        /// </summary>
        [JsonProperty("WD-Flag")]
        public string WdFlag { get; set; }

        /// <summary>
        /// PaRtd 压强
        /// </summary>
        [JsonProperty("PA-Rtd")]
        public decimal? PaRtd { get; set; }

        /// <summary>
        /// PA-Flag
        /// </summary>
        [JsonProperty("PA-Flag")]
        public string PaFlag { get; set; }

        /// <summary>
        /// TEM-Rtd 温度
        /// </summary>
        [JsonProperty("TEM-Rtd")]
        public decimal? TemRtd { get; set; }

        /// <summary>
        /// TEM-Flag
        /// </summary>
        [JsonProperty("TEM-Flag")]
        public string TemFlag { get; set; }

        /// <summary>
        /// TSP-Rtd 总悬浮颗粒物 
        /// </summary>
        [JsonProperty("TSP-Rtd")]
        public decimal? TspRtd { get; set; }

        /// <summary>
        /// TSP-Flag
        /// </summary>
        [JsonProperty("TSP-Flag")]
        public string TspFlag { get; set; }

        /// <summary>
        /// B03-Rtd 噪音
        /// </summary>
        [JsonProperty("B03-Rtd")]
        public decimal? B03Rtd { get; set; }

        /// <summary>
        /// B03-Flag
        /// </summary>
        [JsonProperty("B03-Flag")]
        public string B03Flag { get; set; }

        /// <summary>
        /// RH-Rtd 湿度
        /// </summary>
        [JsonProperty("RH-Rtd")]
        public decimal? RhRtd { get; set; }

        /// <summary>
        /// RH-Flag
        /// </summary>
        [JsonProperty("RH-Flag")]
        public string RhFlag { get; set; }
    }
}
