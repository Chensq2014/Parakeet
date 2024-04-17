namespace Parakeet.Net.ROClient.Models
{
    public class DeviceKeySecret
    {
        /// <summary>
        /// 转发区域
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 供应商授权id，全局唯
        /// </summary>
        public string SupplierKeyId { get; set; }

        /// <summary>
        /// 供应商密钥值，用于请求签名，不在请求中传输，需要妥善保管
        /// </summary>
        public string SupplierKeySecret { get; set; }

        /// <summary>
        /// 项目授权id，全局唯一
        /// </summary>
        public string ProjectKeyId { get; set; }

        /// <summary>
        /// 项目密钥的值，用于请求签名，不在请求中传输，需要妥善保管
        /// </summary>
        public string ProjectKeySecret { get; set; }
    }
}