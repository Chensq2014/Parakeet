//using Parakeet.Net.ServiceGroup.Sign.HttpModels;
//using WebApiClientCore;
//using WebApiClientCore.Attributes;

//namespace Parakeet.Net.ServiceGroup.Sign.HttpApis
//{
//    //[TraceFilter(OutputTarget = OutputTarget.Console)]
//    public interface ISealApi : IHttpApi
//    {
//        /// <summary>
//        /// 创建签章接口
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns>Success</returns>
//        [HttpPost("/tosignserver/api/seal/create")]
//        ITask<SealCreateApiResult> Create([FormContent] SealCreateDto input);

//        /// <summary>
//        /// 签章作废接口
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns>Success</returns>
//        [HttpPost("/tosignserver/api/seal/cancel")]
//        ITask<SignApiResult> Cancel([FormContent] CancelSealDto input);
//    }
//}