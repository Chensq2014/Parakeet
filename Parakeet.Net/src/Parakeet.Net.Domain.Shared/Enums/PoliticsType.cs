using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 政治面貌
    /// </summary>
    [Description("政治面貌")]
    public enum PoliticsType
    {
        /// <summary>
        /// 群众
        /// </summary>
        [Description("群众")]
        群众 = 0,
        /// <summary>
        /// 共青团员
        /// </summary>
        [Description("共青团员")]
        共青团员 = 10,
        /// <summary>
        /// 中共预备党员
        /// </summary>
        [Description("中共预备党员")]
        中共预备党员 = 20,
        /// <summary>
        /// 中共党员
        /// </summary>
        [Description("中共党员")]
        中共党员 = 30
    }
}
