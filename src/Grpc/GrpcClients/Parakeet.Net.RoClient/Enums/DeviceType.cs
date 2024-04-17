using System.ComponentModel;

namespace Parakeet.Net.ROClient
{
    /// <summary>
    /// 环境(1001) | 视频(1002) | 塔吊(1003) | 升降机(1004) | 闸机(1005)，
    /// 人脸采集(1006) | 人员定位(1007) | LED屏(1008) | 运渣车(1009)
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
        /// 塔吊
        /// </summary>
        [Description("塔吊")]
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
        /// 人脸采集
        /// </summary>
        [Description("人脸采集")]
        FaceAcquire = 1006,

        /// <summary>
        /// 车辆管理
        /// </summary>
        [Description("车辆管理")]
        Vehicle = 1009,

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
        /// 卸料平台
        /// </summary>
        [Description("卸料平台")]
        Unloading = 1011,

        /// <summary>
        /// 爬架
        /// </summary>
        [Description("爬架")]
        Climbing = 1012,

        /// <summary>
        /// 基坑监测
        /// </summary>
        [Description("基坑监测")]
        Foundationpit = 2001,

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