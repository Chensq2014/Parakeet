using System;

namespace Parakeet.Net.ROClient.Models
{
    /// <summary>
    /// 设备基础信息
    /// </summary>
    public class DeviceBasic : ModelBase
    {
        public override string CommandName => "add_device_basic";

        /// <summary>
        /// Basic Id
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>
        public Guid? DeviceId { get; set; }

        /// <summary>
        /// 接收设备唯一编码（设备真实设备序列号）
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// 转发设备唯一编码（对外提供设备序列号）
        /// </summary>
        public string FakeNo { get; set; }

        /// <summary>
        /// 记录采集时间
        /// </summary>
        public DateTime? RecordTime { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectCode { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 供应商Id
        /// </summary>
        public string SupplierId { get; set; }

        /// <summary>
        /// 供应商编码
        /// </summary>
        public string SupplierCode { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public decimal? Latitude { get; set; }

        ///// <summary>
        ///// 扩展
        ///// </summary>
        //public string Extend { get; set; }
    }
}