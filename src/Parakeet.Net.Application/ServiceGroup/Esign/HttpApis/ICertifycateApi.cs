//using Parakeet.Net.ServiceGroup.Esign.HttpDtos;
//using WebApiClientCore;
//using WebApiClientCore.Attributes;

//namespace Parakeet.Net.ServiceGroup.Esign.HttpApis
//{
//    /// <summary>
//    /// 个人和组织实名认证接口
//    /// </summary>
//    [TraceFilter]
//    public interface ICertifycateApi : IHttpApi
//    {
//        /// <summary>
//        /// 实名认证三要素接口
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpPost("/openrealname-service/infoauth/psn/telecom")]
//        ITask<ESignApiResult<CertifycateResult>> RealNameValidate([JsonContent] ESignRealNameValidateDto input);

//        /// <summary>
//        /// 企业认证三要素 企业名称 企业统一信用代码 法人
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpPost("/openrealname-service/infoauth/org/identify")]
//        ITask<ESignApiResult<CertifycateResult>> RealNameOrganizationValidate([JsonContent] ESignOrganizationValidateDto input);

//        /// <summary>
//        /// 企业认证二要素 企业统一信用代码
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpPost("/openrealname-service/infoauth/org/simpleIdentity")]
//        ITask<ESignApiResult<CertifycateResult>> RealNameOrganizationSimpleValidate([JsonContent] ESignOrganizationValidateDto input);
//    }
//}
