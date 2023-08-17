using Parakeet.Net.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 安全策略
    /// </summary>
    [Table("Parakeet_SecurePolicies", Schema = "public"),Description("安全策略")]
    public class SecurePolicy : BaseEntity
    {
        public SecurePolicy()
        {
        }

        public SecurePolicy(Guid id)
        {
            base.SetEntityPrimaryKey(id);
        }

        #region 数据源

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255)]
        public string Name { get; set; }
        /// <summary>
        /// 数据源Id
        /// </summary>
        [Description("数据源Id")]
        public Guid? SecureSourceId { get; set; }

        /// <summary>
        /// 数据源类型
        /// None = 0
        /// Role=10
        /// Company=20
        /// Department=30
        /// </summary>
        [Description("数据源类型")]
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
        [Description("安全策略验证类型,验证ip/clientos/browser/deviceId 默认不验证")]
        public SecureValidateType SecureValidateType { get; set; }

        /// <summary>
        /// 特殊Ip地址集
        /// </summary>
        [Description("特殊Ip地址集")]
        public string Ips { get; set; }

        /// <summary>
        /// 开始Ip
        /// </summary>
        [Description("开始Ip")]
        public string StartIpString { get; set; }

        /// <summary>
        /// 结束Ip
        /// </summary>
        [Description("结束Ip")]
        public string EndIpString { get; set; }

        /// <summary>
        /// 开始Ip
        /// </summary>
        [Description("开始Ip")]
        public long StartIpLong { get; set; }

        /// <summary>
        /// 结束Ip
        /// </summary>
        [Description("结束Ip")]
        public long EndIpLong { get; set; }

        /// <summary>
        /// 客户端系统 windows ios andorid
        /// </summary>
        [Description("客户端系统 windows ios andorid")]
        public string ClientOs { get; set; }

        /// <summary>
        /// 移动/PC端浏览器、IE/edge、Chrome
        /// </summary>
        [Description(" 移动/PC端浏览器、IE/edge、Chrome")]
        public string Browser { get; set; }

        /// <summary>
        /// 受信任设备，加域或者安装了客户端管理软件的 设备Id
        /// </summary>
        [Description("受信任设备，加域或者安装了客户端管理软件的 设备Id")]
        public string DeviceId { get; set; }

        #endregion

        #region 启用/禁用 允许/拒绝

        /// <summary>
        /// 启用true/禁用false 默认为false 禁用
        /// </summary>
        [Description("启用true/禁用false 默认为false 禁用")]
        public bool IsEnable { get; set; }

        /// <summary>
        /// 允许true/拒绝false 默认为false 拒绝
        /// </summary>
        [Description("允许true/拒绝false 默认为false 拒绝")]
        public bool IsAllow { get; set; }

        #endregion
    }
}
