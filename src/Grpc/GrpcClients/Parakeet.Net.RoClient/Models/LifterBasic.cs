namespace Parakeet.Net.ROClient.Models
{
    public class LifterBasic : ModelBase
    {
        public override string CommandName => "add_lifter_basic";

        /// <summary>
        /// 升降机吊笼编号
        /// </summary>
        public short? LifterId { get; set; }

        /// <summary>
        /// 升降机名字
        /// </summary>
        /// <value></value>
        public string LiftName { get; set; }

        /// <summary>
        /// 最大载重
        /// </summary>
        public decimal? MaxWeight { get; set; }

        /// <summary>
        /// 最大楼层
        /// </summary>
        public int? MaxFloor { get; set; }

        /// <summary>
        /// 最小楼层
        /// </summary>
        /// <value></value>
        public int? MinFloor { get; set; }

        /// <summary>
        /// 最大高度
        /// </summary>
        public decimal? MaxHeight { get; set; }

        /// <summary>
        /// 最小高度
        /// </summary>
        /// <value></value>
        public decimal? MinHeight { get; set; }

        /// <summary>
        /// 最大速度
        /// </summary>
        public decimal? MaxSpeed { get; set; }

        /// <summary>
        /// 最大倾角
        /// </summary>
        public decimal? MaxTilt { get; set; }

        /// <summary>
        /// 最大风速
        /// </summary>
        /// <value></value>
        public decimal? MaxWindSpeed { get; set; }

        /// <summary>
        /// 最大载人数
        /// </summary>
        public int? MaxPeopleNumber { get; set; }
    }
}