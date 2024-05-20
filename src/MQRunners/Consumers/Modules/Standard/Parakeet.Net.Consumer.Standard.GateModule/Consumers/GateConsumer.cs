using Common.Dtos;
using Common.RabbitMQModule.Core;
using Microsoft.Extensions.DependencyInjection;
using Parakeet.Net.Consumer.Standard.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Parakeet.Net.Consumer.Standard.GateModule.Consumers
{
    /// <summary>
    /// 考勤
    /// </summary>
    public class GateConsumer : ForwardConsumer<GateRecordDto>
    {
        private readonly IGateRecordHttpForward _httpForward;

        public GateConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _httpForward = serviceProvider.GetService<IGateRecordHttpForward>();
            _httpForward.Init(
                source =>
                {
                    source.HttpMethod = HttpMethod.Post;
                    source.HttpClientName = nameof(GateModule);
                });
        }

        protected override QueueInfo QueueInfo => new QueueInfo
        {
            Queue = "gate",
            RoutingKey = "gate"
        };

        protected override string Exchange => "gate";

        protected override async Task EventProcess(WrapperData<GateRecordDto> data)
        {
            await _httpForward.Push(data);
        }

        protected override async Task BatchEventProcess(List<WrapperData<GateRecordDto>> wrapperDataList)
        {
            await _httpForward.BatchPush(wrapperDataList);
        }
    }
}