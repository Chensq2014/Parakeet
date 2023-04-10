using System.ComponentModel;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    ///     枚举辅助dto类
    /// </summary>
    public class EnumTypeItemDto
    {
        public EnumTypeItemDto(int key, string name = "", string description = "")
        {
            ItemKey = key;
            ItemKeyString = key.ToString();
            ItemStringName = name;
            ItemDescription = description;
        }

        /// <summary>
        ///     枚举项整型值
        /// </summary>
        [Description("枚举项整型值")]
        public int ItemKey { get; set; }

        /// <summary>
        ///     枚举项整型值字符串
        /// </summary>
        [Description("枚举项整型值字符串")]
        public string ItemKeyString { get; set; }

        /// <summary>
        ///     枚举项字符串
        /// </summary>
        [Description("枚举项字符串")]
        public string ItemStringName { get; set; }

        /// <summary>
        ///     枚举项描述
        /// </summary>
        [Description("枚举项描述")]
        public string ItemDescription { get; set; }

        /// <summary>
        ///     枚举项描述/枚举项字符串
        /// </summary>
        [Description("枚举项描述/枚举项字符串")]
        public string ItemDescriptionOrName => ItemDescription ?? ItemStringName;
    }
}