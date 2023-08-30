using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 版本 V1 V2 V3
    /// </summary>
    [Description("版本")]
    public enum VersionType
    {
        /// <summary>
        /// 版本1
        /// </summary>
        [Description("v1")]
        V1 = 1,
        /// <summary>
        /// 版本2
        /// </summary>
        [Description("v2")]
        V2 = 2,
        /// <summary>
        /// 版本3
        /// </summary>
        [Description("v3")]
        V3 = 3
    }
}
