namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 宇泛心跳回调数据
    /// </summary>
    public class UfaceIdentityDto
    {
        /// <summary>
        /// 设备唯一标识码
        /// </summary>
        public string DeviceKey { get; set; }

        /// <summary>
        /// 设备当前时间戳
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 设备当前 IP 地址
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 人员Id
        /// </summary>
        public string PersonId { get; set; }

        /// <summary>
        /// face/faceAndcard/idcard/card   0/1/2
        /// face_0（刷脸识别，且该人员在 passtime 内） face_1（刷脸识别，且该人员在 passtime 外）
        /// face_2（刷脸识别/口罩检测，且 识别失败/口罩检测失败）
        /// card_0（刷卡识别，且该人员在passtime 权限时间内 card_1（刷卡识别，且该人员在 passtime 外）
        /// card_2（刷卡识别，且识别失败 faceAndcard_0（双重认证，且该 人员在 passtime权限时间内）
        /// faceAndcard_1（双重认证，且刷 卡结果为该人员在 passtime 权 限时间外）
        /// faceAndcard_2（双重认证，且识 别失败）
        /// idcard_0（人证比对，且该人员 在 passtime 权限时间内）
        /// idcard_1（人证比对，且该人员 在 passtime 权限时间外）
        /// idcard_2（人证比对，且识别失 败）
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string IdNumber { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string IdCardNum { get; set; }

        /// <summary>
        /// 图片Path ftp路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 人脸识别base64图片
        /// </summary>
        public string Base64 { get; set; }

        /// <summary>
        /// 身份证信息
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 人员比对结果
        ///1：比对成功
        ///2：比对失败
        ///3：未进行比对
        /// </summary>
        public string IdentifyType { get; set; }

        /// <summary>
        /// 识别模式
        ///0 刷脸；
        ///1 人脸&卡双重验证；
        ///2 人证比对；
        ///3 刷卡；
        ///4 开门按钮开门；
        ///5 远程开门；
        ///8 口罩检测
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 识别模式
        ///0 刷脸；
        ///1 人脸&卡双重验证；
        ///2 人证比对；
        ///3 刷卡；
        ///4 开门按钮开门；
        ///5 远程开门；
        ///8 口罩检测
        /// </summary>
        public string RecModeType { get; set; }

        /// <summary>
        /// 识别方式 1:本地识别 2:云端识别
        /// </summary>
        public string RecType { get; set; }

        /// <summary>
        /// 活体判断结果1：活体判断成功2：活体判断失败 3：未进行活体判断
        /// </summary>
        public string AliveType { get; set; }

        /// <summary>
        /// 设备AppId
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 人员测量温度值(仅口罩测 温设备支持) 测温模式未打开下，返回 null 测温模式未打开，口罩检测未通 过返回 null
        /// </summary>
        public string Temperature { get; set; }

        /// <summary>
        ///设置的体温异常标准(仅口罩 测温设备支持) 测温模式未打开下，返回 null 测温模式未打开，口罩检测未通 过返回 null
        /// </summary>
        public string Standard { get; set; }

        /// <summary>
        ///温度单位(仅口罩测温设备支持) 1. 摄氏度 2. 华氏度
        /// </summary>
        public string TempUnit { get; set; }
    }
}