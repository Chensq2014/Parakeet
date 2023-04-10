using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    ///  收费类型
    /// </summary>
    [Description("收费类型")]
    public enum ChargeType
    {
        /// <summary>
        /// 铝扣板
        /// </summary>
        [Description("铝扣板")]
        铝扣板 = 10,
        /// <summary>
        /// 风暖
        /// </summary>
        [Description("风暖")]
        风暖 = 20,
        /// <summary>
        /// 防护栏
        /// </summary>
        [Description("防护栏")]
        防护栏 = 30,
        /// <summary>
        /// 扣板异形
        /// </summary>
        [Description("扣板异形")]
        扣板异形 = 40,
        /// <summary>
        /// 拆装扣板
        /// </summary>
        [Description("拆装扣板")]
        拆装扣板 = 41,
        /// <summary>
        /// 人工面积补贴
        /// </summary>
        [Description("人工面积补贴")]
        人工面积补贴 = 50,
        /// <summary>
        /// 暗线
        /// </summary>
        [Description("暗线")]
        暗线 = 60,
        /// <summary>
        /// 硬包
        /// </summary>
        [Description("硬包")]
        硬包 = 70,
        /// <summary>
        /// 硬包基层加宽
        /// </summary>
        [Description("硬包基层加宽")]
        硬包基层加宽 = 72,
        /// <summary>
        /// 包然气管
        /// </summary>
        [Description("包然气管")]
        包然气管 = 71,
        /// <summary>
        /// 石膏线
        /// </summary>
        [Description("石膏线")]
        石膏线 = 80
    }
}
