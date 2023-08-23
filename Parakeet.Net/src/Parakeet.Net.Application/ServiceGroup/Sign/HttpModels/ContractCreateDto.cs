//using System;
//using System.Collections.Generic;
//using WebApiClientCore;

//namespace Parakeet.Net.ServiceGroup.Sign.HttpModels
//{
//    public class ContractCreateDto
//    {
//        /// <summary>
//        /// 业务系统合同编码，系统唯一，后续签署根据此编号完成签署；具体格式和规则由请求方自行定义，但长度不能大于128字符
//        /// </summary>
//        [AliasAs("contract_id")]
//        public string ContractId { get; set; }

//        /// <summary>
//        /// 合同名称
//        /// </summary>
//        [AliasAs("contract_name")]
//        public string ContractName { get; set; }

//        /// <summary>
//        /// 设置该合同需要几方签署,至多10方签署
//        /// </summary>
//        [AliasAs("party_count")]
//        public int PartyCount { get; set; }

//        /// <summary>
//        /// 针对本次请求的唯一标识，系统唯一，业务流水号的具体格式和规则由请求方自行定义，但长度不能大于128字符
//        /// </summary>
//        [AliasAs("biz_id")]
//        public string BizId { get; set; }

//        /// <summary>
//        /// 发起合同的业务系统处理时间，格式为yyyy-MM-dd HH:mm:ss
//        /// </summary>
//        [AliasAs("biz_time")]
//        public DateTime BizTime { get; set; }

//        ///// <summary>
//        ///// 此参数和file_url必须含有至少其一，合同内的文档，可以上传多个文件，支持上传的pdf/xml/office格式文件，系统会根据文件类型自动判断非PDF文档，自动转换成PDF
//        ///// </summary>
//        //[AliasAs("doc_file")]
//        //public MulitpartFile DocFile { get; set; }

//        /// <summary>
//        /// HTTP文件下载地址，请在资源链接后拼接file_name参数，值为文件名称（包含后缀）
//        /// </summary>
//        [AliasAs("file_url")]
//        public string FileUrl { get; set; }

//        ///// <summary>
//        ///// 需要添加到合同内的附件文件,默认该参数的文档将作为附件添加至第一个文档，如果需要将附件附加到其他文档，参数名称需要添加文档索引，比如"attach_file_2"的参数的文档将附加到合同的第二个文档内
//        ///// </summary>
//        //[AliasAs("attach_file")]
//        //public MulitpartFile AttachFile  { get; set; }

//        ///// <summary>
//        ///// 上传图片作为签署文件
//        ///// </summary>
//        //[AliasAs("picture_file")]
//        //public MulitpartFile PictureFile { get; set; }

//        /// <summary>
//        /// 如果上传的XML格式模板文件需要动态填充数据，需要在该参数设置填充的实际json值,如果上传多个XML模板文件的话，需设置多个xml_metadata，系统自动根据参数顺序自动填充
//        ///格式为json格式字符串 ， 例如：{"姓名":"徐荣","cardname_ID":"360111198708080899","year":"2018","month":"01","day":"01"}
//        /// </summary>
//        [AliasAs("xml_metadata")]
//        public string XmlMetadata { get; set; }

//        /// <summary>
//        /// 如果上传的DOCX格式模板文件需要动态填充数据，需要在该参数设置填充的实际json值，可以将多个文档的文档内的填充数据合并为一个json，例如：{"姓名":"徐荣","cardname_ID":"360111198708080899","year":"2018","month":"01","day":"01"}
//        /// </summary>
//        [AliasAs("doc_metadata")]
//        public string DocMetadata { get; set; }

//        /// <summary>
//        /// 在发起的合同文档添加二维码水印信息，默认这些参数的文档添加到所有文档，如果需要指定不同的水印添加到不同的文档，参数名称需要添加文档索引，比如qrcode_2的参数将添加到合同的第二个文档内；JSON格式字符串
//        ///例如：{"metaname":"二维码名称","x":"99%","textval":"www.tosign.cn","y":"99%"}
//        /// </summary>
//        [AliasAs("qrcode")]
//        public QrCode QrCode { get; set; }

//        /// <summary>
//        /// 在发起的合同文档添加图片水印，默认这些参数的文档添加到所有文档，如果需要指定不同的水印添加到不同的文档，参数名称需要添加文档索引，比如watermarkimage_2的参数将添加到合同的第二个文档内；JSON格式字符串
//        ///例如：{"x":"99%","y":"1%","metaname":"文字水印","imgData":"base64图片"}
//        /// </summary>
//        [AliasAs("watermarkimage")]
//        public WaterMarkImage WaterMarkImage { get; set; }

//        /// <summary>
//        /// 在发起的合同文档添加文本水印，默认这些参数的文档添加到所有文档，如果需要指定不同的水印添加到不同的文档，参数名称需要添加文档索引，比如watermarktext_2的参数将添加到合同的第二个文档内；JSON格式字符串
//        ///例如：{"metaname":"文字水印","color":"#1873c9","x":"50%","y":"50%","fontsize":"40","textval":"电子合同部","basefont":"STXINGKA.TTF"}
//        /// </summary>
//        [AliasAs("watermarktext")]
//        public WaterMarkText WaterMarkText { get; set; }
//    }

//    public class ContractCreateApiResult : SignApiResult
//    {
//        /// <summary>
//        /// 业务流水号，原样返回请求方传递的业务流水号
//        /// </summary>
//        [AliasAs("biz_id")]
//        public string BizId { get; set; }

//        /// <summary>
//        /// 合同编码，原样返回请求方传递的合同编码
//        /// </summary>
//        [AliasAs("contract_id")]
//        public string ContractId { get; set; }

//        /// <summary>
//        /// 合同ID，由信签服务器生成的合同ID
//        /// </summary>
//        [AliasAs("cnt_id")]
//        public string CntId { get; set; }

//        /// <summary>
//        /// 合同内所有文档源文件下载地址
//        /// </summary>
//        [AliasAs("doc_original_urls")]
//        public List<string> DocOriginalUrls { get; set; }

//        /// <summary>
//        /// 合同内所有文档最后签署文档下载地址
//        /// </summary>
//        [AliasAs("doc_last_urls")]
//        public List<string> DocLastUrls { get; set; }

//        /// <summary>
//        /// 合同内所有的文档ID
//        /// </summary>
//        [AliasAs("documentids")]
//        public List<string> DocumentIds { get; set; }

//        /// <summary>
//        /// 合同内单个文件查看地址
//        /// </summary>
//        [AliasAs("document_view_urls")]
//        public List<string> DocumentViewUrls { get; set; }

//        /// <summary>
//        /// 合同内所有的文档图片下载地址
//        /// </summary>
//        [AliasAs("doc_image_urls")]
//        public List<string> DocImageUrls { get; set; }

//        /// <summary>
//        /// 合同签署页面地址，用于需要在浏览器完成签署操作，需要加上签署人的相关信息，详细说明参考本章节的《浏览器合同签署》接口
//        /// </summary>
//        [AliasAs("doc_sign_url")]
//        public string DocSignUrl { get; set; }

//        /// <summary>
//        /// 合同查阅页面地址，用于在浏览器查阅合同
//        /// </summary>
//        [AliasAs("doc_view_url")]
//        public string DocViewUrl { get; set; }
//    }
//}