//using Parakeet.Net.ServiceGroup.Esign.HttpDtos;
//using WebApiClientCore;
//using WebApiClientCore.Attributes;

//namespace Parakeet.Net.ServiceGroup.Esign.HttpApis
//{
//    /// <summary>
//    /// 代理E签签章接口
//    /// </summary>
//    [TraceFilter]
//    public interface IESignApi : IHttpApi
//    {
//        [HttpPost("/tech-sdkwrapper/timevale/init")]
//        ITask<ESignApiResult> Init([JsonContent] InitInputDto input);

//        [HttpPost("/tech-sdkwrapper/timevale/sign/userStreamSign")]
//        ITask<SignResult> UserStreamSign(MulitpartFile file,
//            [MulitpartText] string fileName,
//            [MulitpartText] string signPos,
//            [MulitpartText] string signType,
//            [MulitpartText] string accountId,
//            [MulitpartText] string sealData
//        );
//    }
//}
