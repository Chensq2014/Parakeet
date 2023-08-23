//using WebApiClientCore;

//namespace Parakeet.Net.ServiceGroup.Sign.HttpModels
//{
//    public class RegisterDto
//    {
//        /// <summary>
//        /// 签署用户编码，唯一标识
//        /// </summary>
//        [AliasAs("signer_code")]
//        public string Code { get; set; }

//        /// <summary>
//        /// 签署用户名称
//        /// </summary>
//        [AliasAs("signer_name")]
//        public string Name { get; set; }

//        /// <summary>
//        /// 签署用户手机号码
//        /// </summary>
//        [AliasAs("signer_phone")]
//        public string Phone { get; set; }

//        /// <summary>
//        /// 签署用户类型 ，默认值：1， 可选【1：个人用户，2：企业用户】
//        /// </summary>
//        [AliasAs("signer_type")]
//        public int? Type { get; set; }

//        /// <summary>
//        /// 签署用户证件类型 ，默认值：0，
//        ///可选【0：居民身份证，1：护照，B：港澳居民往来内地通行证，C：台湾居民来往大陆通行证，
//        ///2：社会信用代码，3：工商登记证，4：税务登记证，7：组织机构代码证，8：企业营业执照 ，Z：其他】
//        /// </summary>
//        [AliasAs("signer_idtype")]
//        public string IdType { get; set; }

//        /// <summary>
//        /// 签署用户证件编号
//        /// </summary>
//        [AliasAs("signer_idcode")]
//        public string IdCode { get; set; }

//        /// <summary>
//        /// 签署用户证书类型 ，不传递参数不创建， 可选【1：文件证书，2：普通证书,4：事件证书】
//        /// </summary>
//        [AliasAs("signer_certtype")]
//        public int? CertType { get; set; }

//        /// <summary>
//        /// 当证书类型参数为文件证书时，需要上传base64文件证书
//        /// </summary>
//        [AliasAs("signer_certdata")]
//        public string CertData { get; set; }

//        /// <summary>
//        /// 当证书类型参数为文件证书时，设置文件证书的密码
//        /// </summary>
//        [AliasAs("signer_certpwd")]
//        public string CertPassword { get; set; }
//    }
//}