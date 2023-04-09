using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 安全策略验证类型
    /// None = 0
    /// Ip=10
    /// ClientOs = 20
    /// Browser=30
    /// DeviceId = 40
    /// </summary>
    [Description("安全策略验证类型")]
    public enum SecureValidateType
    {
        /// <summary>
        /// 未设置None 默认不验证
        /// </summary>
        [Description("未设置")]
        None = 0,
        /// <summary>
        /// 验证Ip
        /// </summary>
        [Description("验证Ip")]
        Ip = 10,
        /// <summary>
        /// 客户端操作系统ClientOs
        /// </summary>
        [Description("客户端操作系统ClientOs")]
        ClientOs = 20,
        /// <summary>
        /// 客户端浏览器Browser
        /// </summary>
        [Description("客户端浏览器Browser")]
        Browser = 30,
        /// <summary>
        /// 信任设备DeviceId
        /// </summary>
        [Description("信任设备DeviceId")]
        DeviceId = 40
    }
}
