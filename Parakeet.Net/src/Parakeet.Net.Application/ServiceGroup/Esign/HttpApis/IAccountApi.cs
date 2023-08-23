//using Parakeet.Net.ServiceGroup.Esign.HttpDtos;
//using WebApiClientCore;
//using WebApiClientCore.Attributes;

//namespace Parakeet.Net.ServiceGroup.Esign.HttpApis
//{
//    /// <summary>
//    /// 注册个人和组织账号
//    /// </summary>
//    //[TraceFilter]
//    public interface IAccountApi : IHttpApi
//    {
//        /// <summary>
//        /// 个人账号注册
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpPost("/tech-sdkwrapper/timevale/account/addPerson")]
//        ITask<AddPersonApiResult> AddPerson([JsonContent] PersonBeanDto input);

//        /// <summary>
//        /// 组织账号注册
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpPost("/tech-sdkwrapper/timevale/account/addOrganize")]
//        ITask<AddPersonApiResult> AddOrganize([JsonContent] OrganizeBeanDto input);
//    }
//}