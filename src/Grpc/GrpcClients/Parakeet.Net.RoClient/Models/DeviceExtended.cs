namespace Parakeet.Net.ROClient.Models
{
    public class DeviceExtended
    {
        /// <summary>
        /// 转发区域
        /// PS:不一定是设备所在区域,有可能是其他平台的编码
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
        
        /// <summary>
        /// 标签
        /// </summary>
        public string Tags { get; set; }
    }
}