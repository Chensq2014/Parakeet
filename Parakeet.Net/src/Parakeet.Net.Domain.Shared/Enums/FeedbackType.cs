using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 设备反馈类型
    /// </summary>
    [Description("设备反馈类型")]
    public enum FeedbackType
    {
        /// <summary>
        /// 人员删除成功
        /// </summary>
        [Description("人员删除成功")]
        人员删除成功 = 0,
        /// <summary>
        /// 人员删除失败
        /// </summary>
        [Description("人员删除失败")]
        人员删除失败 = 1,
        /// <summary>
        /// 人员下发成功
        /// </summary>
        [Description("人员下发成功")]
        人员下发成功 = 2,
        /// <summary>
        /// 人员下发失败
        /// </summary>
        [Description("人员下发失败")]
        人员下发失败 = 3
    }
}
