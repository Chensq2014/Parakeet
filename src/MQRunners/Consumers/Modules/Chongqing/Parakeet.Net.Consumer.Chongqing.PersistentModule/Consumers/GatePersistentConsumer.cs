using Common.Dtos;
using Common.Entities;
using Common.RabbitMQModule.Core;
using System;

namespace Parakeet.Net.Consumer.Chongqing.PersistentModule.Consumers
{
    /// <summary>
    /// 考勤持久化消费者
    /// </summary>
    public class GatePersistentConsumer : DataCenterConsumer<ChongqingPersistentModule, GateBase, GateRecordDto>
    {
        public GatePersistentConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override QueueInfo QueueInfo => new QueueInfo
        {
            Queue = "test",
            RoutingKey = "test"
        };
        protected override string Exchange => "Gate";
    }
}
