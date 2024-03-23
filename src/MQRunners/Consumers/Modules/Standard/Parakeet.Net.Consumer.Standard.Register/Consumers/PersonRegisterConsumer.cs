using Common.Dtos;
using Common.RabbitMQModule.Core;
using Microsoft.Extensions.DependencyInjection;
using Parakeet.Net.Consumer.Standard.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Parakeet.Net.Consumer.Standard.RegisterModule.Consumers
{
    /// <summary>
    ///standard模块 注册人员消费者
    /// </summary>
    public class PersonRegisterConsumer : ForwardConsumer<DeviceWorkerDto>
    {
        private readonly IPersonRegisterHttpForward _httpForward;
        public PersonRegisterConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _httpForward = serviceProvider.GetRequiredService<IPersonRegisterHttpForward>();
            _httpForward.Init(source =>
            {
                source.HttpMethod = HttpMethod.Post;
                source.HttpClientName = nameof(RegisterModule);
            });
        }

        protected override QueueInfo QueueInfo => new QueueInfo
        {
            Queue = "test",
            RoutingKey = "test"
        };
        protected override string Exchange => "register";
        protected override async Task EventProcess(WrapperData<DeviceWorkerDto> wrapperData)
        {
            await _httpForward.Push(wrapperData);
        }

        protected override async Task BatchEventProcess(List<WrapperData<DeviceWorkerDto>> wrapperDataList)
        {
            await _httpForward.BatchPush(wrapperDataList);
        }
    }
}
