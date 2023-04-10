using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    ///     级别:0、中国；1、省/直辖市/自治区；2、市；3、区/县；4、街道；5、村
    /// </summary>
    [Description("地区类型")]
    public enum DeepLevelType
    {
        /// <summary>
        ///     国家
        /// </summary>
        [Description("国家")] Country = 0,

        /// <summary>
        ///     省/直辖市/自治区
        /// </summary>
        [Description("省/直辖市")] Province = 1,

        /// <summary>
        ///     城市(市辖区/县)
        /// </summary>
        [Description("城市(市辖区)")] City = 2,

        /// <summary>
        ///     市辖区/县
        /// </summary>
        [Description("区/县")] Area = 3,

        /// <summary>
        ///     街道/乡/镇
        /// </summary>
        [Description("街道/乡/镇")] Street = 4,

        /// <summary>
        ///     村
        /// </summary>
        [Description("村")] Village = 5
    }
}