namespace Parakeet.Net.ROClient.Models
{
    public class CraneBasic : ModelBase
    {
        public override string CommandName => "add_crane_basic";

        /// <summary>
        /// 塔机在当前塔群中的编号
        /// </summary>
        /// <value></value>
        public short? CraneId { get; set; }

        /// <summary>
        /// 小臂长
        /// </summary>
        /// <value></value>
        public decimal? ShortArm { get; set; }

        /// <summary>
        /// 大臂长
        /// </summary>
        /// <value></value>
        public decimal? LongArm { get; set; }

        /// <summary>
        /// 塔帽高/塔高 单位0.1m 2byte
        /// </summary>
        public decimal? TowerHatHeight { get; set; }

        /// <summary>
        /// 起重臂高(塔臂高) 单位0.1m 2byte
        /// </summary>
        public decimal? BoomHeight { get; set; }

        /// <summary>
        /// 最大吊重 2byte
        /// </summary>
        public decimal? MaxLoadWeight { get; set; }

        /// <summary>
        /// 最大力矩 2byte
        /// </summary>
        public decimal? MaxTorque { get; set; }

        /// <summary>
        /// 吊钩重量 2byte
        /// </summary>
        public decimal? HookWeight { get; set; }

        /// <summary>
        /// 塔机转角为0时与正北夹角，以右为正.范围：0-360
        /// </summary>
        public decimal? CompassAngle { get; set; }

        /// <summary>
        /// 坐标x 单位0.1m 2byte
        /// </summary>
        public decimal? X { get; set; }

        /// <summary>
        /// 坐标y 单位0.1m 2byte
        /// </summary>
        public decimal? Y { get; set; }

        /// <summary>
        /// 吊绳倍率
        /// </summary>
        /// <value></value>
        public int? Fall { get; set; }
    }
}