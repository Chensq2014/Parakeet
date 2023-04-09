using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    ///处理类型
    /// </summary>
    public enum MediatorHandlerType
    {
        /// <summary>
        /// record
        /// </summary>
        [Description("record")]
        Record = 0,

        /// <summary>
        /// heartbeat
        /// </summary>
        [Description("heartbeat")]
        Heartbeat = 10,

        /// <summary>
        /// feedback
        /// </summary>
        [Description("feedback")]
        Feedback = 20
    }
}
