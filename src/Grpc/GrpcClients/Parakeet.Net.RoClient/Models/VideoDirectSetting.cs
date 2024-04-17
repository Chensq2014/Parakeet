namespace Parakeet.Net.ROClient
{
    public class VideoDirectSetting : ModelBase
    {
        public override string CommandName => "send_video_direct";

        /// <summary>
        /// 指令类型
        /// </summary>
        public VideoDirectType DirectType { get; set; } = VideoDirectType.GOTO_PRESET;

        /// <summary>
        /// 数值(如飞行速度,移动速度等)
        /// 取值范围[1,7]
        /// </summary>
        public int Parameter { get; set; } = 1;
    }
}