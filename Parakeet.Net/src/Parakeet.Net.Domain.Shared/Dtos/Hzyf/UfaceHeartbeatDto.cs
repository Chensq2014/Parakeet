namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 宇泛心跳回调数据
    /// </summary>
    public class UfaceHeartbeatDto
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
        /// 设备当前注册人员数量
        /// </summary>
        public string PersonCount { get; set; }

        /// <summary>
        /// 设备当前注册的照片数量
        /// </summary>
        public string FaceCount { get; set; }

        /// <summary>
        /// 设备版本号
        /// </summary>
        public string Version { get; set; }
    }
}