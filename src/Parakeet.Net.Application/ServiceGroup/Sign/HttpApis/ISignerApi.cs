//using Parakeet.Net.ServiceGroup.Sign.HttpModels;
//using WebApiClientCore;
//using WebApiClientCore.Attributes;

//namespace Parakeet.Net.ServiceGroup.Sign.HttpApis
//{
//    //[TraceFilter]
//    public interface ISignerApi : IHttpApi
//    {
//        /// <summary>
//        /// 注册用户接口
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns>Success</returns>
//        [HttpPost("/tosignserver/api/signer/register")]
//        ITask<SignApiResult> Register([FormContent] RegisterDto input);

//        /// <summary>
//        /// 实名认证三要素接口
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpPost("/tosignserver/api/signer/realNameAuthenticateByMobile")]
//        ITask<RealNameAuthApiResult> RealNameValidate([FormContent] RealNameValidateDto input);

//        /// <summary>
//        /// 实名认证四要素接口
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpPost("/tosignserver/api/signer/realNameAuth")]
//        ITask<RealNameAuthApiResult> RealNameAuth([FormContent] RealNameAuthDto input);

//        /// <summary>
//        /// 企业打款验证申请接口
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpPost("/tosignserver/api/signer/enterpriseCheckApply")]
//        ITask<EnterpriseCheckApplyApiResult> EnterpriseCheckApply([FormContent] EnterpriseCheckApplyDto input);

//        /// <summary>
//        /// 企业打款验证申请接口
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpPost("/tosignserver/api/signer/enterpriseCheck")]
//        ITask<EnterpriseCheckApiResult> EnterpriseCheck([FormContent] EnterpriseCheckDto input);
        
//    }
//}