namespace Parakeet.Net.ROClient
{
    public enum FPSensorType
    {
        /// <summary>
        /// 全站仪机器人
        /// </summary>
        TotalStation = 0X01,

        /// <summary>
        /// 固定式测斜仪
        /// </summary>
        FixInclino = 0X02,

        /// <summary>
        /// 钢筋计
        /// </summary>
        Reinforcement = 0X03,

        /// <summary>
        /// 裂缝计
        /// </summary>
        Crack = 0X04,

        /// <summary>
        /// 锚索计
        /// </summary>
        AnchorCableDynamo = 0X05,

        /// <summary>
        /// 渗压计
        /// </summary>
        Osmo = 0X06,

        /// <summary>
        /// 土压力计
        /// </summary>
        EarthPressure = 0X07,

        /// <summary>
        /// 测振仪
        /// </summary>
        Vibro = 0X08,

        /// <summary>
        /// 倾角计
        /// </summary>
        Tilt = 0X09,

        /// <summary>
        /// 静力水准仪
        /// </summary>
        StaticLevel = 0X10,

        /// <summary>
        /// 轴力计
        /// </summary>
        AxialForce = 0X11,

        /// <summary>
        ///  应变计
        /// </summary>
        Strain = 0X12,

        /// <summary>
        /// 水位计
        /// </summary>
        WaterLevel = 0X13
    }
}