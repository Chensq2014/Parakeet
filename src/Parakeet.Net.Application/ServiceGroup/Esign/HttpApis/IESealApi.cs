//using Parakeet.Net.ServiceGroup.Esign.HttpDtos;
//using WebApiClientCore;
//using WebApiClientCore.Attributes;

//namespace Parakeet.Net.ServiceGroup.Esign.HttpApis
//{
//    /// <summary>
//    /// E签注册个人/组织 印章
//    /// </summary>
//    [TraceFilter]
//    public interface IESealApi : IHttpApi
//    {
//        /// <summary>
//        /// 创建用户个性签章
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpPost("/tech-sdkwrapper/timevale/seal/addPersonSeal")]
//        ITask<SealApiResult> AddPersonSeal([JsonContent] AddPersonSealInput input);

//        /// <summary>
//        /// 创建企业签章
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpPost("/tech-sdkwrapper/timevale/seal/addOrganizeSeal")]
//        ITask<SealApiResult> AddOrganizeSeal([JsonContent] OrganizeSealInput input);
//    }
//}
