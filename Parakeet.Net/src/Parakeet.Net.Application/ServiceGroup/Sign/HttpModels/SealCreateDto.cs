//using WebApiClientCore;

//namespace Parakeet.Net.ServiceGroup.Sign.HttpModels
//{
//    public class SealCreateDto
//    {
//        /// <summary>
//        /// 绑定用户编号
//        /// </summary>
//        [AliasAs("signer_code")]
//        public string SignerCode { get; set; }

//        /// <summary>
//        /// 签章编号，唯一
//        /// </summary>
//        [AliasAs("seal_code")]
//        public string SealCode { get; set; }

//        /// <summary>
//        /// 签章名称
//        /// </summary>
//        [AliasAs("seal_name")]
//        public string SealName { get; set; }

//        /// <summary>
//        /// 是否默认;默认为非默认状态，(1)为默认 ,
//        /// </summary>
//        [AliasAs("seal_isdefault")]
//        public string SealIsDefault { get; set; }

//        /// <summary>
//        /// 签章类型 ：业务专用章(1),财务专用章(2),发票专用章(3);可为逗号分隔的字符串
//        /// </summary>
//        [AliasAs("seal_types")]
//        public string SealTypes { get; set; }

//        /// <summary>
//        /// 签章图片base64;如果传递图片则默认根据图片生成签章
//        /// </summary>
//        [AliasAs("seal_img")]
//        public string SealImg { get; set; }

//        /// <summary>
//        /// 签章图片高度（cm）
//        /// </summary>
//        [AliasAs("seal_height")]
//        public string SealHeight { get; set; }

//        /// <summary>
//        /// 签章图片宽度（cm）
//        /// </summary>
//        [AliasAs("seal_width")]
//        public string SealWidth { get; set; }

//        /// <summary>
//        /// 签章样式：人名方章(1)，企业圆章(2);不传递签章图片则根据签章样式生成签章
//        /// </summary>
//        [AliasAs("seal_form")]
//        public string SealForm { get; set; }

//        /// <summary>
//        /// 防伪数字，企业圆章下弦月的文字内容
//        /// </summary>
//        [AliasAs("seal_licenseno")]
//        public string SealLicenseNo { get; set; }
//    }

//    public class SealCreateApiResult : SignApiResult
//    {
//        /// <summary>
//        /// 实际签章宽度：单位（px）
//        /// </summary>
//        [AliasAs("seal_width")]
//        public string SealWidth { get; set; }

//        /// <summary>
//        /// 实际签章高度：单位（px）
//        /// </summary>
//        [AliasAs("seal_height")]
//        public string SealHeight { get; set; }

//        /// <summary>
//        /// 签章数据
//        /// </summary>
//        [AliasAs("seal_data")]
//        public string SealData { get; set; }

//        /// <summary>
//        /// 签章图片格式
//        /// </summary>
//        [AliasAs("seal_fmt")]
//        public string SealFmt { get; set; }

//        /// <summary>
//        /// 签章图片名称
//        /// </summary>
//        [AliasAs("seal_name")]
//        public string SealName { get; set; }

//        /// <summary>
//        /// 结果信息
//        /// </summary>
//        [AliasAs("message")]
//        public string Message { get; set; }

//        /// <summary>
//        /// 用户编号
//        /// </summary>
//        [AliasAs("signer_code")]
//        public string SignerCode { get; set; }

//        /// <summary>
//        /// 签章编号
//        /// </summary>
//        [AliasAs("seal_code")]
//        public string SealCode { get; set; }

//        /// <summary>
//        /// 签章宽度：单位（CM）
//        /// </summary>
//        [AliasAs("seal_cm_width")]
//        public decimal SealCmWidth { get; set; }

//        /// <summary>
//        /// 签章高度：单位（CM）
//        /// </summary>
//        [AliasAs("seal_cm_height")]
//        public decimal SealCmHeight { get; set; }
//    }
//}