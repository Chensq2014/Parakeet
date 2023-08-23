using Parakeet.Net.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 更新安全策略传输对象
    /// </summary>
    public class SecurePolicyUpdateDto : EntityDto<Guid>
    {
        #region 数据源

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255)]
        public string Name { get; set; }

        /// <summary>
        /// 数据源Id
        /// </summary>
        public Guid? SecureSourceId { get; set; }

        /// <summary>
        /// Org数据源类型
        /// None = 0
        /// Role=10
        /// Company=20
        /// Department=30
        /// </summary>
        public SecureSourceType SecureSourceType { get; set; }

        #endregion

        #region 规则

        /// <summary>
        /// 安全策略验证类型,验证ip/clientos/browser/deviceId 默认不验证
        /// None = 0
        /// Ip=10
        /// ClientOs = 20
        /// Browser=30
        /// DeviceId = 40
        /// </summary>
        public SecureValidateType SecureValidateType { get; set; }

        /// <summary>
        /// 特殊Ip地址集
        /// </summary>
        public string Ips { get; set; }

        /// <summary>
        /// 开始Ip
        /// </summary>
        public string StartIpString { get; set; }

        /// <summary>
        /// 结束Ip
        /// </summary>
        public string EndIpString { get; set; }

        /// <summary>
        /// 开始Ip
        /// </summary>
        public long StartIpLong { get; set; }

        /// <summary>
        /// 结束Ip
        /// </summary>
        public long EndIpLong { get; set; }

        /// <summary>
        /// 客户端系统 windows ios andorid
        /// </summary>
        public string ClientOs { get; set; }

        /// <summary>
        /// 移动/PC端浏览器、IE/edge、Chrome
        /// </summary>
        public string Browser { get; set; }

        /// <summary>
        /// 受信任设备，加域或者安装了客户端管理软件的 设备Id
        /// </summary>
        public string DeviceId { get; set; }

        #endregion

        #region 启用/禁用 允许/拒绝

        /// <summary>
        /// 启用true/禁用false 默认为false 禁用
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 允许true/拒绝false 默认为false 拒绝
        /// </summary>
        public bool IsAllow { get; set; }

        #endregion
    }
}
