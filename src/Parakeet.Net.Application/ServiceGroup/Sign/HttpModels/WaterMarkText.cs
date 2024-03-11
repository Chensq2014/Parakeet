//using WebApiClientCore;

//namespace Parakeet.Net.ServiceGroup.Sign.HttpModels
//{
//    public class WaterMarkText
//    {
//        /// <summary>
//        /// 水印名称
//        /// </summary>
//        [AliasAs("metaname")]
//        public string MetaName { get; set; }

//        /// <summary>
//        /// 文本水印
//        /// </summary>
//        [AliasAs("textval")]
//        public string TextVal { get; set; }

//        /// <summary>
//        /// 二维码放置X坐标位置，以文档页面左边为起点向右偏移x位置 (支持像素和百分比设置)
//        /// 例如:x:”50”或者 x:”50%”
//        /// </summary>
//        [AliasAs("x")]
//        public string X { get; set; }

//        /// <summary>
//        /// 二维码放置Y坐标位置，以文档页面左边为起点向右偏移y位置 (支持像素和百分比设置)
//        ///例如:y:”50”或者y:”50%”
//        /// </summary>
//        [AliasAs("y")]
//        public string Y { get; set; }

//        /// <summary>
//        /// 16进制RGB颜色类型，例如#00ff00。（默认#000000，纯黑）
//        /// </summary>
//        [AliasAs("color")]
//        public string Color { get; set; }

//        /// <summary>
//        /// 文字大小，默认40
//        /// </summary>
//        [AliasAs("fontsize")]
//        public string FontSize { get; set; }

//        /// <summary>
//        /// 文字字体：默认：SIMLI.TTF ，可选：MSYH.TTF (微软雅黑)，STXINGKA.TTF（行楷），SIMSUN.TTC（宋体），SIMLI.TTF（隶书），SIMKAI.TTF（楷体）
//        /// </summary>
//        [AliasAs("basefont")]
//        public string BaseFont { get; set; }

//        /// <summary>
//        /// 文字旋转角度,默认：0，可选【0、90、180、270】
//        /// </summary>
//        [AliasAs("rotation")]
//        public string Rotation { get; set; }

//        /// <summary>
//        /// 是否跨页，默认：0，可选【0：不跨页，1：跨页】
//        /// </summary>
//        [AliasAs("crossPage")]
//        public string CrossPage { get; set; }

//        /// <summary>
//        /// 水印透明度，默认不透明，选址范围：0~100的值，100为不透明，默认：20
//        /// </summary>
//        [AliasAs("fillOpacity")]
//        public string FillOpacity { get; set; }
//    }
//}