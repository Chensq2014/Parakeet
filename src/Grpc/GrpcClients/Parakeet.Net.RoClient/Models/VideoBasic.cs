namespace Parakeet.Net.ROClient.Models
{
    public class VideoBasic : ModelBase
    {
        public override string CommandName => "add_video_record";

        /// <summary>
        /// 转发编码
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// 转发设备唯一编码（对外提供设备序列号）
        /// </summary>
        public string FakeNo { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 摄像头编号，视频播放会以camNo排序，且系统默认camNo最小的序号为项目主摄像头。原则主摄像头应覆盖项目全貌
        /// </summary>
        public int CameraNo { get; set; } = 1;

        /// <summary>
        /// 拍摄区域
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 视频封面图片
        /// </summary>
        public string CoverUrl { get; set; }

        /// <summary>
        /// 可直接播放的视频流地址，如果该项有值平台会直接取该地址进行播放
        /// </summary>
        public string Source { get; set; }
    }
}