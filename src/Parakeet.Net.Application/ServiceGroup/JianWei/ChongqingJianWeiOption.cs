using System.ComponentModel;
using Common.Dtos;

namespace Parakeet.Net.ServiceGroup.JianWei
{
    /// <summary>
    ///JianWei账号密钥
    /// </summary>
    public class ChongqingJianWeiOption : BaseAppOptionDto
    {
        /// <summary>
        /// 供应商授权id，全局唯一
        /// </summary>
        [Description("供应商授权id，全局唯一")]
        public string SupplierKeyId { get; set; }

        /// <summary>
        /// 供应商密钥值，用于请求签名，不在请求中传输，需要妥善保管
        /// </summary>
        [Description("供应商密钥值，用于请求签名，不在请求中传输，需要妥善保管")]
        public string SupplierKeySecret { get; set; }

        /// <summary>
        /// 项目授权id，全局唯一
        /// </summary>

        [Description("项目授权id，全局唯一")]
        public string ProjectKeyId { get; set; }

        /// <summary>
        /// 项目密钥的值，用于请求签名，不在请求中传输，需要妥善保管
        /// </summary>
        [Description("项目密钥的值，用于请求签名，不在请求中传输，需要妥善保管")]
        public string ProjectKeySecret { get; set; }

        /// <summary>
        /// 同步数据用的设备编码FakeNo
        /// </summary>
        [Description("同步数据用的设备编码FakeNo")]
        public string FakeNo { get; set; }
    }
}
