using Microsoft.Extensions.DependencyInjection;
using Common.Dtos;
using Parakeet.Net.XiamenHuizhan;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Parakeet.Net.BackgroundWorks
{
    /// <summary>
    /// 厦门会展请求服务 url会根据设备编码等规则变化，使用httpClient动态请求
    /// </summary>
    public class XiamenHuizhanBackGroundWorker : PeriodicBackgroundWorkerBase
    {
        public XiamenHuizhanBackGroundWorker(AbpTimer timer, IServiceScopeFactory serviceScopeFactory) : base(timer, serviceScopeFactory)
        {
            Timer.Period = 600000; //10分钟一次轮询
        }

        protected override void DoWork(PeriodicBackgroundWorkerContext workerContext)
        {
            AsyncHelper.RunSync(async () =>
            {
                //一标段环境 
                await SectionOneEnvironmentRealTimeRequest(new RequestParameterDto { }).ConfigureAwait(false);
                //二标段环境
                await SectionTwoEnvironmentRealTimeRequest(new RequestParameterDto { }).ConfigureAwait(false);
                //一标段考勤历史记录
                await SectionOneGateHistoryRequest(new RequestParameterDto { }).ConfigureAwait(false);
            });
        }

        #region 一标段

        /// <summary>
        /// 一标段环境设备实时数据请求
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task SectionOneEnvironmentRealTimeRequest(RequestParameterDto input)
        {
            var returnObj = await SectionExtention.SectionOneEnvironmentRealTimeRequest(input);
            if (returnObj?.Data?.Data != null)
            {
                //await _environmentRecordAppService.CreateSectionOneEnvironmentRecord(returnObj).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// 一标段考勤历史记录请求
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task SectionOneGateHistoryRequest(RequestParameterDto input)
        {
            var returnObj = await SectionExtention.SectionOneGateHistoryRequest(input);
            if (returnObj?.Data?.Data != null && returnObj.Data.Data.Any())
            {
                //await _gateRecordAppService.CreateSectionOneGateRecords(returnObj);
                //继续请求.一直到请求无数据为止
                input.Next++;
                await SectionOneGateHistoryRequest(input);
            }
        }


        #endregion

        #region 二标段

        /// <summary>
        /// 二标段环境设备实时数据请求
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task SectionTwoEnvironmentRealTimeRequest(RequestParameterDto input)
        {
            var returnObj = await SectionExtention.SectionTwoEnvironmentRealTimeRequest(input);
            if (returnObj?.Content?.RealtimeData != null)
            {
                //await _environmentRecordAppService.CreateSectionTwoEnvironmentRecord(returnObj).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// 2标段考勤历史记录请求
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task SectionTwoGateHistoryRequest(RequestParameterDto input)
        {
            var returnObj = await SectionExtention.SectionTwoGateHistoryRequest(input);
            if (returnObj?.Data != null && returnObj.Data.Any())
            {
                //await _gateRecordAppService.CreateSectionTwoGateRecords(returnObj);
                //继续请求.一直到请求无数据为止
                input.Next = returnObj.Data.OrderBy(m => m.Date).LastOrDefault()?.Id ?? ++input.Next;//input.Next++;
                await SectionTwoGateHistoryRequest(input);
            }
        }

        #endregion
    }
}
