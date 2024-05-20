using Common.Dtos;
using Common.Entities;
using Common.RabbitMQModule.Core;
using System;

namespace Parakeet.Net.Consumer.Chongqing.PersistentModule.Consumers
{
    /// <summary>
    /// 环境持久化消费者
    /// </summary>
    public class EnvironmentPersistentConsumer : DataCenterConsumer<ChongqingPersistentModule, EnvironmentBase, EnvironmentRecordDto>
    {
        public EnvironmentPersistentConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override QueueInfo QueueInfo => new QueueInfo
        {
            Queue = "test",
            RoutingKey = "test"
        };
        protected override string Exchange => "Environment";
    }
}
