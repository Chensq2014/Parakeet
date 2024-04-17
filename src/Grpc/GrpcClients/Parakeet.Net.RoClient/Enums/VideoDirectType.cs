namespace Parakeet.Net.ROClient
{
    /// <summary>
    /// 视频云台指令类型
    /// </summary>
    public enum VideoDirectType
    {
        /// <summary>
        /// 接通灯光电源
        /// </summary>
        LIGHT_PWRON = 2,

        /// <summary>
        /// 接通雨刷开关
        /// </summary>
        WIPER_PWRON = 3,

        /// <summary>
        /// 接通风扇开关
        /// </summary>
        FAN_PWRON = 4,

        /// <summary>
        /// 接通加热器开关
        /// </summary>
        HEATER_PWRON = 5,

        /// <summary>
        /// 接通辅助设备开关
        /// </summary>
        AUX_PWRON1 = 6,

        /// <summary>
        /// 接通辅助设备开关
        /// </summary>
        AUX_PWRON2 = 7,

        /// <summary>
        /// 设置预置点
        /// </summary>
        SET_PRESET = 8,

        /// <summary>
        /// 清除预置点
        /// </summary>
        CLE_PRESET = 9,

        /// <summary>
        /// 焦距以速度SS变大(倍率变大)
        /// </summary>
        ZOOM_IN = 11,

        /// <summary>
        /// 焦距以速度SS变小(倍率变小)
        /// </summary>
        ZOOM_OUT = 12,

        /// <summary>
        /// 焦点以速度SS前调
        /// </summary>
        FOCUS_NEAR = 13,

        /// <summary>
        /// 焦点以速度SS后调
        /// </summary>
        FOCUS_FAR = 14,

        /// <summary>
        /// 光圈以速度SS扩大
        /// </summary>
        IRIS_OPEN = 15,

        /// <summary>
        /// 光圈以速度SS缩小
        /// </summary>
        IRIS_CLOSE = 16,

        /// <summary>
        /// 云台以SS的速度上仰
        /// </summary>
        TILT_UP = 21,

        /// <summary>
        /// 云台以SS的速度下俯
        /// </summary>
        TILT_DOWN = 22,

        /// <summary>
        /// 云台以SS的速度左转
        /// </summary>
        PAN_LEFT = 23,

        /// <summary>
        /// 云台以SS的速度右转
        /// </summary>
        PAN_RIGHT = 24,

        /// <summary>
        /// 云台以SS的速度上仰和左转
        /// </summary>
        UP_LEFT = 25,

        /// <summary>
        /// 云台以SS的速度上仰和右转
        /// </summary>
        UP_RIGHT = 26,

        /// <summary>
        /// 云台以SS的速度下俯和左转
        /// </summary>
        DOWN_LEFT = 27,

        /// <summary>
        /// 云台以SS的速度下俯和右转
        /// </summary>
        DOWN_RIGHT = 28,

        /// <summary>
        /// 云台以SS的速度左右自动扫描
        /// </summary>
        PAN_AUTO = 29,

        /// <summary>
        /// 将预置点加入巡航序列
        /// </summary>
        FILL_PRE_SEQ = 30,

        /// <summary>
        /// 设置巡航点停顿时间
        /// </summary>
        SET_SEQ_DWELL = 31,

        /// <summary>
        /// 设置巡航速度
        /// </summary>
        SET_SEQ_SPEED = 32,

        /// <summary>
        /// 将预置点从巡航序列中删除
        /// </summary>
        CLE_PRE_SEQ = 33,

        /// <summary>
        /// 开始记录轨迹
        /// </summary>
        STA_MEM_CRUISE = 34,

        /// <summary>
        /// 停止记录轨迹
        /// </summary>
        STO_MEM_CRUISE = 35,

        /// <summary>
        /// 开始轨迹
        /// </summary>
        RUN_CRUISE = 36,

        /// <summary>
        /// 开始巡航
        /// </summary>
        RUN_SEQ = 37,

        /// <summary>
        /// 停止巡航
        /// </summary>
        STOP_SEQ = 38,

        /// <summary>
        /// 快球转到预置点
        /// </summary>
        GOTO_PRESET = 39
    }
}