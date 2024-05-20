using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace ConsoleApp.Dtos
{
    public class KeySecret : EntityDto<Guid>
    {
        /// <summary>
        /// 供应商授权id，全局唯
        /// </summary>
        public virtual string SupplierKeyId { get; set; }

        /// <summary>
        /// 供应商密钥值，用于请求签名，不在请求中传输，需要妥善保管
        /// </summary>
        public virtual string SupplierKeySecret { get; set; }

        /// <summary>
        /// 项目授权id，全局唯一
        /// </summary>
        public virtual string ProjectKeyId { get; set; }

        /// <summary>
        /// 项目密钥的值，用于请求签名，不在请求中传输，需要妥善保管
        /// </summary>
        public virtual string ProjectKeySecret { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public virtual string DeviceCode { get; set; }
    }
}
