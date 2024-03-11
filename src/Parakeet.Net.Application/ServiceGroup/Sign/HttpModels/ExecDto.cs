//using System;
//using System.Collections.Generic;
//using WebApiClientCore;

//namespace Parakeet.Net.ServiceGroup.Sign.HttpModels
//{
//    public class ExecDto
//    {
//        /// <summary>
//        /// 业务系统合同编码，系统唯一，后续签署根据此编号完成签署；具体格式和规则由请求方自行定义，但长度不能大于128字符
//        /// </summary>
//        [AliasAs("contract_id")]
//        public string ContractId { get; set; }

//        /// <summary>
//        /// 针对本次请求的唯一标识，系统唯一，业务流水号的具体格式和规则由请求方自行定义，但长度不能大于128字符；需要注意如果签署失败，重试请将原业务流水号传递过来，所以签署前请先保存。
//        /// </summary>
//        [AliasAs("biz_id")]
//        public string BizId { get; set; }

//        /// <summary>
//        /// 发起合同的业务系统处理时间，格式为yyyy-MM-dd HH:mm:ss
//        /// </summary>
//        [AliasAs("biz_time")]
//        public DateTime BizTime { get; set; }

//        /// <summary>
//        /// 业务备注
//        /// </summary>
//        [AliasAs("biz_remark")]
//        public string BizRemark { get; set; }

//        /// <summary>
//        /// 在调用《创建签署用户》指定的签署用户编码
//        /// </summary>
//        [AliasAs("signer_code")]
//        public string SignerCode { get; set; }

//        /// <summary>
//        /// 指定签署位置信息，JSON格式字符串，例如：[{ y:123,x:100,pageno:"1"}]
//        /// </summary>
//        [AliasAs("signatures")]
//        public string Signatures { get; set; }

//        /// <summary>
//        /// 指定用户的某个签章，如果为空，将获取用户的默认签章执行盖章
//        /// </summary>
//        [AliasAs("seal_code")]
//        public string SealCode { get; set; }

//        /// <summary>
//        /// 设置签署的信息，在签署属性原因项查看
//        /// </summary>
//        [AliasAs("reason")]
//        public string Reason { get; set; }

//        /// <summary>
//        /// 其他附加签署人信息，设置除了流程外的签署人外的其他盖章位置信息，可以提交多个参数，完成多人签署
//        /// </summary>
//        [AliasAs("signer_info")]
//        public string SignerInfo { get; set; }

//        /// <summary>
//        /// 印章水印属性属性
//        /// </summary>
//        [AliasAs("textinfo")]
//        public string TextInfo { get; set; }
//    }

//    public class ExecApiResult : SignApiResult
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
//        /// 合同查看签署地址
//        /// </summary>
//        [AliasAs("doc_view_url")]
//        public string DocViewUrl { get; set; }

//        /// <summary>
//        /// 返回合同第几次签署
//        /// </summary>
//        [AliasAs("lastparty")]
//        public int LastParty { get; set; }

//        /// <summary>
//        /// 返回本地签署的用户编码
//        /// </summary>
//        [AliasAs("signer_code")]
//        public string SignerCode { get; set; }

//        /// <summary>
//        /// 合同内所有文档最初源文件下载地址
//        /// </summary>
//        [AliasAs("doc_original_urls")]
//        public List<string> DocOriginalUrls { get; set; }

//        /// <summary>
//        /// 本次签署合同内的文档的签署前下载地址
//        /// </summary>
//        [AliasAs("record_input_urls")]
//        public List<string> RecordInputUrls { get; set; }

//        /// <summary>
//        /// 本次签署合同内的文档的签署后下载地址
//        /// </summary>
//        [AliasAs("record_saved_urls")]
//        public List<string> RecordSavedUrls { get; set; }

//        /// <summary>
//        /// 签署前图片地址
//        /// </summary>
//        [AliasAs("record_image_ori_urls")]
//        public List<string> RecordImageOriUrls { get; set; }

//        /// <summary>
//        /// 本次签署的文档图片下载地址
//        /// </summary>
//        [AliasAs("record_image_urls")]
//        public List<string> RecordImageUrls { get; set; }

//        /// <summary>
//        /// 存证证书下载地址，注：在完成合同签署之后，请等待至少30分钟后再调用该接口，以防获取证书失败。
//        /// </summary>
//        [AliasAs("evidence_query_urls")]
//        public List<string> EvidenceQueryUrls { get; set; }
//    }
//}