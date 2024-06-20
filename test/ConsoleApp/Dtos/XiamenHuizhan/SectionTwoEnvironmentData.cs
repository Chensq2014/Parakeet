using System;
using System.Collections.Generic;

namespace ConsoleApp.Dtos.XiamenHuizhan
{
    /// <summary>
    /// SectionTwoEnvironmentReturnData
    /// </summary>
    public class SectionTwoEnvironmentReturnData
    {
        /// <summary>
        /// Code
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 接口调用成功
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Content
        /// </summary>
        public SectionTwoEnvironmentContent Content { get; set; }
    }

    /// <summary>
    /// SectionTwoEnvironmentContent
    /// </summary>
    public class SectionTwoEnvironmentContent
    {
        /// <summary>
        /// DeviceId
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 在线
        /// </summary>
        public string Live { get; set; }

        /// <summary>
        /// DateTime
        /// </summary>
        public DateTime? DateTime { get; set; }

        /// <summary>
        /// RealtimeData
        /// </summary>
        public List<SectionTwoEnvironmentData> RealtimeData { get; set; }
    }


    /// <summary>
    /// SectionTwoEnvironmentData
    /// </summary>
    public class SectionTwoEnvironmentData
    {
        /// <summary>
        /// Sensor对照字段
        /// PM10:a34002
        /// PM2.5:a34004
        /// 噪声:a50001
        /// 风速:a01007
        /// 风向:a01008
        /// 温度:a01001
        /// 湿度:a01002
        /// </summary>
        public string Sensor { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Flag
        /// </summary>
        public string Flag { get; set; }

        /// <summary>
        /// Data
        /// </summary>
        public decimal? Data { get; set; }

        /// <summary>
        /// Max
        /// </summary>
        public decimal? Max { get; set; }

        /// <summary>
        /// Min
        /// </summary>
        public decimal? Min { get; set; }
    }
}
