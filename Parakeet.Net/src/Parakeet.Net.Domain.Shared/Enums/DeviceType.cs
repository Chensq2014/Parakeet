using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 环境(1001) | 视频(1002) | 起重机(1003) | 升降机(1004) | 闸机(1005)，
    /// 运渣车(1006) | 实名制(1007) | LED屏(1008) | 噪音(1009) | 安全帽(1010)
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// 环境
        /// </summary>
        [Description("环境")]
        Environment = 1001,

        /// <summary>
        /// 视频
        /// </summary>
        [Description("视频")]
        Video = 1002,

        /// <summary>
        /// 起重机
        /// </summary>
        [Description("起重机")]
        Crane = 1003,

        /// <summary>
        /// 升降机
        /// </summary>
        [Description("升降机")]
        Lifter = 1004,

        /// <summary>
        /// 闸机
        /// </summary>
        [Description("闸机")]
        Gate = 1005,

        /// <summary>
        /// 运渣车
        /// </summary>
        [Description("运渣车")]
        SlagCar = 1006,

        /// <summary>
        /// 人员定位
        /// </summary>
        [Description("人员定位")]
        PersonLocation = 1007,

        /// <summary>
        /// LED屏
        /// </summary>
        [Description("LED屏")]
        LED = 1008,

        /// <summary>
        /// 噪音
        /// </summary>
        [Description("噪音")]
        Noise = 1009,

        /// <summary>
        /// 安全帽
        /// </summary>
        [Description("安全帽")]
        Helmet = 1010,

        /// <summary>
        /// 基坑监测
        /// </summary>
        [Description("基坑监测")]
        Foundationpit = 2000,

        /// <summary>
        /// 基坑-全站仪测量机器人
        /// </summary>
        [Description("基坑-全站仪测量机器人")]
        FP_Totalstation = 2001,

        /// <summary>
        /// 基坑-固定式测斜仪器
        /// </summary>
        [Description("基坑-固定式测斜仪器")]
        FP_FixInclinometer = 2002,

        /// <summary>
        /// 基坑-钢筋计
        /// </summary>
        [Description("基坑-钢筋计")]
        FP_Reinforcementmeter = 2003,

        /// <summary>
        /// 基坑-裂缝检测仪
        /// </summary>
        [Description("基坑-裂缝检测仪")]
        FP_Crackmeter = 2004,

        /// <summary>
        /// 基坑-锚索测力计
        /// </summary>
        [Description("基坑-锚索测力计")]
        FP_AnchorCableDynamometer = 2005,

        /// <summary>
        /// 基坑-渗压计
        /// </summary>
        [Description("基坑-渗压计")]
        FP_Osmometer = 2006,

        /// <summary>
        /// 基坑-土压计
        /// </summary>
        [Description("基坑-土压计")]
        FP_EarthPressuremeter = 2007,

        /// <summary>
        /// 基坑-VF测振动仪
        /// </summary>
        [Description("基坑-VF测振动仪")]
        FP_Vibrometer = 2008,

        /// <summary>
        /// 基坑-倾角计
        /// </summary>
        [Description("基坑-倾角计")]
        FP_Tiltmeter = 2009,

        /// <summary>
        /// 基坑-静力水准仪
        /// </summary>
        [Description("基坑-静力水准仪")]
        FP_StaticLevelmeter = 2010,

        /// <summary>
        /// 水表
        /// </summary>
        [Description("水表")]
        Watermeter = 3001,

        /// <summary>
        /// 电表
        /// </summary>
        [Description("电表")]
        Electricitymeter = 3002,

        /// <summary>
        /// 气表
        /// </summary>
        [Description("气表")]
        Gasmeter = 3003,

        /// <summary>
        /// 地磅
        /// </summary>
        [Description("地磅")]
        Weighbridge = 3010
    }
}
