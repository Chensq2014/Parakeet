using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    ///     枚举: 水印位置
    /// </summary>
    public enum WatermarkPosition
    {
        /// <summary>
        ///     左上
        /// </summary>
        [Description("左上")]
        LeftTop = 0,

        /// <summary>
        ///     左中
        /// </summary>
        [Description("左中")]
        Left = 10,

        /// <summary>
        ///     左下
        /// </summary>
        [Description("左下")]
        LeftBottom = 20,

        /// <summary>
        ///     正上
        /// </summary>
        [Description("正上")]
        Top = 30,

        /// <summary>
        ///     正中
        /// </summary>
        [Description("正中")]
        Center = 40,

        /// <summary>
        ///     正下
        /// </summary>
        [Description("正下")]
        Bottom = 50,

        /// <summary>
        ///     右上
        /// </summary>
        [Description("右上")]
        RightTop = 60,

        /// <summary>
        ///     右中
        /// </summary>
        [Description("右中")]
        RightCenter = 70,

        /// <summary>
        ///     右下
        /// </summary>
        [Description("右下")]
        RigthBottom = 80,

        /// <summary>
        ///     全屏
        /// </summary>
        [Description("全屏")]
        Cover = 200
    }

}
