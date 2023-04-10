using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 设备用户状态(未下发/已下发)
    /// </summary>
    [Description("设备用户状态(激活/禁用)")]
    public enum DeviceWorkerState
    {
        /// <summary>
        /// 未下发
        /// </summary>
        [Description("未下发")]
        UnSendToDevice = 0,
        /// <summary>
        /// 已下发
        /// </summary>
        [Description("已下发")]
        SendToDevice = 10
    }
}
