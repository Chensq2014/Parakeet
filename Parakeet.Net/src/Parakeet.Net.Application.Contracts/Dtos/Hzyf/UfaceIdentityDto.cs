using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 宇泛心跳回调数据
    /// </summary>
    public class UfaceIdentityDto
    {
        /// <summary>
        /// 设备唯一标识码
        /// </summary>
        public string DeviceKey { get; set; }

        /// <summary>
        /// 设备当前时间戳
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 设备当前 IP 地址
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 人员Id
        /// </summary>
        public string PersonId { get; set; }

        /// <summary>
        /// face/card   0/1/2
        /// </summary>
        public string Type { get; set; }

    }
}
