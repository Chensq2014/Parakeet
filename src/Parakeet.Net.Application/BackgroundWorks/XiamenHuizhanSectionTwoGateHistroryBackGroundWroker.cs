using Microsoft.Extensions.DependencyInjection;
using Common.Dtos;
using Parakeet.Net.XiamenHuizhan;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Parakeet.Net.BackgroundWorks
{
    /// <summary>
    /// 厦门会展二标段考勤历史记录请求服务 url会根据设备编码等规则变化，使用httpClient动态请求
    /// </summary>
    public class XiamenHuizhanSectionTwoGateHistroryBackGroundWroker : PeriodicBackgroundWorkerBase
    {
        public XiamenHuizhanSectionTwoGateHistroryBackGroundWroker(AbpTimer timer, IServiceScopeFactory serviceScopeFactory) : base(timer, serviceScopeFactory)
        {
            Timer.Period = 86400000; //86400000; //每天一次轮询
        }

        protected override void DoWork(PeriodicBackgroundWorkerContext workerContext)
        {
            AsyncHelper.RunSync(async () =>
            {
                //二标段考勤历史记录 一次请求就已经是1天的了因为时间参数是yyyy-MM-dd的形式  这个只能每天运行一次
                await SectionTwoGateHistoryRequest(new RequestParameterDto
                {
                    //InOutType = "IN",
                    StartDate = DateTime.Now.AddDays(-1),
                    EndDate = DateTime.Now.AddDays(-1)
                }).ConfigureAwait(false);
                ////二标段考勤历史记录 一次请求就已经是1天的了因为时间参数是yyyy-MM-dd的形式  这个只能每天运行一次
                //await SectionTwoGateHistoryRequest(new RequestParameterDto
                //{
                //    //InOutType = "OUT",
                //    StartDate = DateTime.Now.AddDays(-1),
                //    EndDate = DateTime.Now.AddDays(-1)
                //}).ConfigureAwait(false);
            });
        }
        #region 二标段

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
                //依赖注入应用层服务接口 调用保存数据操作
                //await _gateRecordAppService.CreateSectionTwoGateRecords(returnObj);
                //继续请求.一直到请求无数据为止
                input.Next = returnObj.Data.OrderBy(m => m.Date).LastOrDefault()?.Id ?? ++input.Next;//input.Next++;
                await SectionTwoGateHistoryRequest(input);
            }
        }

        #endregion
    }
}
