using Parakeet.Net.ServiceGroup.JianWei.HttpDtos;
using System.Collections.Generic;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace Parakeet.Net.ServiceGroup.JianWei.HttpApis
{
    /// <summary>
    /// 重庆建委V1.0 Api
    /// </summary>
    //[TraceFilter(OutputTarget = OutputTarget.Console)]
    public interface IChongqingJianWeiApi : IHttpApi
    {
        /// <summary>
        /// 人员信息下载接口
        /// </summary>
        /// <param name="page">请求页面，首次请求可以不传，page从0开始计算，首页page=0,第二页page=1,依次类推</param>
        /// <returns></returns>
        [HttpGet("api/open/rn/v3/projectWorker")]
        ITask<JianWeiResult<IList<ProjectWorkerDto>>> GetProjectWorker(int page);
    }
}